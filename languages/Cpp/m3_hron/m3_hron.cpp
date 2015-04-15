// ----------------------------------------------------------------------------------------------
// Copyright (c) Mårten Rånge.
// ----------------------------------------------------------------------------------------------
// This source code is subject to terms and conditions of the Microsoft Public License. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// If you cannot locate the  Microsoft Public License, please send an email to 
// dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
//  by the terms of the Microsoft Public License.
// ----------------------------------------------------------------------------------------------
// You must not remove this notice, or any other, from this software.
// ----------------------------------------------------------------------------------------------

// -----------------------------------------------------------------------------
#include "stdafx.h"
// -----------------------------------------------------------------------------
#pragma warning(disable:4100)
#pragma warning(disable:4512)
// -----------------------------------------------------------------------------
#include <Windows.h>
// -----------------------------------------------------------------------------
#include <cassert>
#include <string>
#include <vector>
// -----------------------------------------------------------------------------
#include "hron_parser.hpp"
// -----------------------------------------------------------------------------
namespace
{

    template<typename TPredicate>
    struct scope_guard
    {
        TPredicate predicate;

        scope_guard (scope_guard && sg) throw ()
            :   predicate (std::move(sg.predicate))
        {
        }

        scope_guard (TPredicate const & predicate) throw ()
            :   predicate (predicate)
        {
        }

        scope_guard (TPredicate && predicate) throw ()
            :   predicate (std::move(predicate))
        {
        }

        ~scope_guard () throw ()
        {
            predicate ();
        }

    private:
        scope_guard (scope_guard const &);
        scope_guard& operator= (scope_guard const &);
        scope_guard& operator= (scope_guard const &&);
    };

    template <typename TPredicate>
    scope_guard<TPredicate> on_exit (TPredicate&& predicate) throw ()
    {
        return scope_guard<TPredicate> (std::forward<TPredicate> (predicate));
    }

    struct exitcode_exception : std::exception 
    {
        int const exitcode;

        exitcode_exception (int exitcode)
            :   exitcode (exitcode)
        {
        }
    };

    bool directory_exists (std::wstring const & path)
    {
        auto fa = GetFileAttributes (path.c_str ());
        return 
                fa != INVALID_FILE_ATTRIBUTES 
            &&  (fa & FILE_ATTRIBUTE_DIRECTORY);
    }

    std::wstring get__reference_datum_path ()
    {
        wchar_t raw_path[MAX_PATH] = {};
        GetModuleFileNameW(nullptr, raw_path, MAX_PATH);

        std::wstring path (raw_path);
        auto f = path.find_last_of (L'\\');
        if (f != std::wstring::npos)
        {
            path = path.substr(0, f + 1) + L"..\\..\\..\\..\\reference-data\\";
        }

        return path;
    }

    std::vector<std::wstring> get__reference_datum (std::wstring const & reference_data_path)
    {
        auto search_for = reference_data_path + L"*.hron"; 
        WIN32_FIND_DATA ff = {};
        auto handle = FindFirstFileW (search_for.c_str (), &ff);
        if (handle == INVALID_HANDLE_VALUE)
        {
            return std::vector<std::wstring> ();
        }

        auto close_handle = on_exit ([=](){FindClose(handle);});

        std::vector<std::wstring> result;

        do
        {
            result.push_back (ff.cFileName);
        }
        while (FindNextFileW(handle, &ff));

        return result;
    }

    struct visitor_state
    {
        std::ofstream   output  ;
    };

    void error (void* payload, int line_no, hron_string_type line, int b, int e, hron_string_type message)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Error:" << std::string (line + b, line + e) << std::endl;
    }

    void preprocessor (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "PreProcessor:" << std::string (s + b, s + e) << std::endl;
    }

    void empty (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Empty:" << std::string (s + b, s + e) << std::endl;
    }

    void comment (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output 
            << "Comment:" 
            << b - 1
            << ","
            << std::string (s + b, s + e) 
            << std::endl
            ;
    }

    void object__begin (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Object_Begin:" << std::string (s + b, s + e) << std::endl;
    }

    void object__end (void * payload)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Object_End:" << std::endl;
    }

    void value__begin (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Value_Begin:" << std::string (s + b, s + e) << std::endl;
    }

    void value__line (void * payload, hron_string_type s, int b, int e)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "ContentLine:" << std::string (s + b, s + e) << std::endl;
    }


    void value__end (void * payload)
    {
        visitor_state & vs = *reinterpret_cast<visitor_state*> (payload);

        vs.output << "Value_End:" << std::endl;
    }

    static bool test_bom (std::ifstream & input)
    {
        return 
                input.peek () != 0xEF
            ||  (
                    input.get() == 0xEF
                &&  input.get() == 0xBB
                &&  input.get() == 0xBF
                )
            ;
    }

    struct console__visitor : hron::i__visitor
    {
        static std::wstring get         (hron_string_type b, hron_string_type e)
        {
            assert(b);
            assert(e);
            assert(b <= e);
            return std::wstring (b,e);
        }

        virtual void    document__begin (){wprintf(L"document__begin\r\n");}
        virtual void    document__end   (){wprintf(L"document__end\r\n");}

        virtual void    preprocessor    (hron_string_type b, hron_string_type e){wprintf(L"preprocessor:%s\r\n", get(b,e).c_str ());}
        virtual void    comment         (hron_string_type b, hron_string_type e){wprintf(L"comment:%s\r\n", get(b,e).c_str ());}
        virtual void    empty           (hron_string_type b, hron_string_type e){wprintf(L"empty:%s\r\n", get(b,e).c_str ());}

        virtual void    object__begin   (hron_string_type b, hron_string_type e){wprintf(L"object__begin:%s\r\n", get(b,e).c_str ());}
        virtual void    object__end     (){wprintf(L"object__end\r\n");}

        virtual void    value__begin    (hron_string_type b, hron_string_type e){wprintf(L"value__begin:%s\r\n", get(b,e).c_str ());}
        virtual void    value__line     (hron_string_type b, hron_string_type e){wprintf(L"value__line:%s\r\n", get(b,e).c_str ());}
        virtual void    value__end      (){wprintf(L"value__end\r\n");}

        virtual void    error           (int line_no, hron_string_type b, hron_string_type e, hron_string_type msg){wprintf(L"error\r\n");}
    };
    
    void unit_test (
            std::wstring const & reference_datum_path
        ,   std::wstring const & action_logs_path
        )
    {
        wprintf(L"1. Testing units\r\n");

        auto large_path = reference_datum_path + L"helloworld.hron";

        wprintf(L"Looking for %s\r\n", large_path.c_str());

        std::ifstream hron (large_path);
        std::vector<std::string> lines;
        std::string line;


        if (!hron)
        {
            wprintf(L"Failed to open stream\r\n");
            return;
        }

        if (!test_bom (hron))
        {
            wprintf(L"Failed to open stream, need to be UTF-8\r\n");
            return;
        }

        while (std::getline (hron, line))
        {
            lines.push_back (line);
        }

        console__visitor cv;

        {
            hron::parser p (&cv);
            for (auto line : lines)
            {
                p.accept_line (line);
            }
        }

        {
            hron::parser p (&cv);
        }
    }

    void correctness_test (
            std::wstring const & reference_datum_path
        ,   std::wstring const & action_logs_path
        )
    {
        wprintf(L"2. Testing correctness\r\n");

        auto reference_datum = get__reference_datum (reference_datum_path);

        visitor_state vs;
        hron__visitor v = {};
        v.payload = &vs;
        v.error             = error             ;

        v.preprocessor      = preprocessor      ;
        v.empty             = empty             ;
        v.comment           = comment           ;

        v.object__begin     = object__begin     ;
        v.object__end       = object__end       ;

        v.value__begin      = value__begin      ;
        v.value__line       = value__line       ;
        v.value__end        = value__end        ;

        for(auto reference_data : reference_datum)
        {
            wprintf(L"Processing %s\r\n", reference_data.c_str());

            auto reference_data_path = reference_datum_path + reference_data;
            auto action_log_path = action_logs_path + reference_data + L".actionlog";

            std::ifstream hron (reference_data_path);

            if (!hron)
            {
                wprintf(L"Failed to open stream\r\n");
                continue;
            }

            if (!test_bom (hron))
            {
                wprintf(L"Failed to open stream, need to be UTF-8\r\n");
                continue;
            }

            vs.output = std::ofstream(action_log_path);

            auto parser_state = hron__initialize(&v);
            if (parser_state)
            {
                auto close_parser = on_exit ([=](){hron__finalize (parser_state);});

                std::string line;

                while (std::getline(hron, line))
                {
                    hron__accept_line (parser_state, line.c_str (), 0, static_cast<int> (line.size ()));
                }
            }
        }

    }

    void performance_test (
            std::wstring const & reference_datum_path
        ,   std::wstring const & action_logs_path
        )
    {
        wprintf(L"3. Testing performance\r\n");

        auto large_path = reference_datum_path + L"large.hron";

        wprintf(L"Looking for %s\r\n", large_path.c_str());

        std::ifstream hron (large_path);
        std::vector<std::string> lines;
        std::string line;


        if (!hron)
        {
            wprintf(L"Failed to open stream\r\n");
            return;
        }

        if (!test_bom (hron))
        {
            wprintf(L"Failed to open stream, need to be UTF-8\r\n");
            return;
        }

        while (std::getline (hron, line))
        {
            lines.push_back (line);
        }

        auto then = GetTickCount() + 0x0ui64;
        int count = 100u;

        hron__visitor dv = {};

        for (auto iter = 0; iter < count; ++iter)
        {
            auto parser_state = hron__initialize(&dv);
            if (parser_state)
            {
                auto close_parser = on_exit ([=](){hron__finalize (parser_state);});

                for (auto line : lines)
                {
                    hron__accept_line (parser_state, line.c_str (), 0, static_cast<int> (line.size ()));
                }
            }
        }

        auto now = GetTickCount() + 0x100000000ui64;
        auto diff = static_cast<__int32> ((now - then) & 0xFFFFFFFF);

        wprintf(
                L"%d lines reading %d milliseconds\r\n"
            ,   count * lines.size()
            ,   diff
            );

    }
}

// -----------------------------------------------------------------------------
int main()
{
    try
    {
        auto reference_datum_path = get__reference_datum_path ();
        auto action_logs_path = reference_datum_path + L"test-results\\C\\";

        wprintf(L"m3_hron - running test cases\r\n");

        if (!directory_exists (reference_datum_path))
        {
            wprintf(L"Exiting due to missing reference datum directory (%s)\r\n", reference_datum_path.c_str ());
        }

        if (!directory_exists (action_logs_path))
        {
            wprintf(L"Exiting due to missing action log directory (%s)\r\n", action_logs_path.c_str ());
        }

        unit_test           (reference_datum_path, action_logs_path);
        correctness_test    (reference_datum_path, action_logs_path);
        performance_test    (reference_datum_path, action_logs_path);

        return 0;
    }
    catch (exitcode_exception const & e)
    {
        return e.exitcode;
    }
    catch (std::exception const & e)
    {
        wprintf(L"Exiting due to error: %S\r\n", e.what ()); 
        return 999;
    }
    catch (...)
    {
        wprintf(L"Exiting due to unrecognized error\r\n"); 
        return 999;
    }
}
// -----------------------------------------------------------------------------

