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
#include <Windows.h>
// -----------------------------------------------------------------------------
#include <string>
#include <vector>
// -----------------------------------------------------------------------------
extern "C"
{
    #include "hron_parser.h"
}
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
        // TODO:
        return L"C:\\temp\\GitHub\\hron\\reference-data\\";
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

}
// -----------------------------------------------------------------------------
int main()
{
    try
    {
        auto reference_datum_path = get__reference_datum_path ();
        auto action_log_path = reference_datum_path + L"test-results\\C\\";

        if (!directory_exists (reference_datum_path))
        {
            wprintf(L"Exiting due to missing reference datum directory (%s)\r\n", reference_datum_path.c_str ());
        }

        if (!directory_exists (action_log_path))
        {
            wprintf(L"Exiting due to missing action log directory (%s)\r\n", reference_datum_path.c_str ());
        }

        auto reference_datum = get__reference_datum (reference_datum_path);

        visitor_state vs;
        hron__visitor v = {};
        v.payload = &vs;

        v.object__begin     = object__begin     ;
        v.object__end       = object__end       ; 

        for(auto reference_data : reference_datum)
        {
            wprintf(L"Processing %s\r\n", reference_data.c_str());

            auto reference_data_path = reference_datum_path + reference_data;

            std::ifstream hron (reference_data_path);

            if (!hron)
            {
                wprintf(L"Failed to open stream%s\r\n");
                continue;
            }

            auto parser_state = hron__initialize(&v);
            if (parser_state)
            {
                auto close_parser = on_exit ([=](){hron__finalize (parser_state);});

                std::string line;

                while (std::getline(hron, line))
                {
                    hron__accept_line (parser_state, line.c_str (), 0, line.size ());
                }
            }
            break;
        }



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

