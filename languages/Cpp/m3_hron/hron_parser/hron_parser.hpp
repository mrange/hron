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
#pragma once
// -----------------------------------------------------------------------------
#include <cassert>
#include <map>
#include <stack>
#include <string>
// -----------------------------------------------------------------------------
extern "C"
{
    #include "hron_parser.h"
}
// -----------------------------------------------------------------------------
namespace hron
{
    struct i__visitor
    {
        virtual void    document__begin () = 0;
        virtual void    document__end   () = 0;

        virtual void    preprocessor    (hron_string_type b, hron_string_type e) = 0;
        virtual void    comment         (hron_string_type b, hron_string_type e) = 0;
        virtual void    empty           (hron_string_type b, hron_string_type e) = 0;

        virtual void    object__begin   (hron_string_type b, hron_string_type e) = 0;
        virtual void    object__end     () = 0;

        virtual void    value__begin    (hron_string_type b, hron_string_type e) = 0;
        virtual void    value__line     (hron_string_type b, hron_string_type e) = 0;
        virtual void    value__end      () = 0;

        virtual void    error           (int line_no, hron_string_type b, hron_string_type e, hron_string_type msg) = 0;
    };

    struct parser
    {
        parser (i__visitor * visitor) throw ()
            :   state (nullptr)
        {
            assert (visitor);

            hron__visitor v = {};

            v.payload             = visitor                     ;
            v.document__begin     = &parser::document__begin    ;
            v.document__end       = &parser::document__end      ;
            v.preprocessor        = &parser::preprocessor       ;
            v.comment             = &parser::comment            ;
            v.empty               = &parser::empty              ;
            v.object__begin       = &parser::object__begin      ;
            v.object__end         = &parser::object__end        ;
            v.value__begin        = &parser::value__begin       ;
            v.value__line         = &parser::value__line        ;
            v.value__end          = &parser::value__end         ;
            v.error               = &parser::error              ;
            
            state = hron__initialize (&v);            
        }

        parser (parser && p) throw ()
            :   state (p.state)
        {
            p.state = nullptr;
        }

        parser& operator= (parser && p) throw ()
        {
            state = p.state;
            p.state = nullptr;
        }

        ~parser () throw ()
        {
            hron__finalize (state);            
        }

        inline void accept_line (hron_string_type b, hron_string_type e) throw ()
        {
            assert(b);
            assert(e);
            assert(e >= b);
            assert(e - b <= INT_MAX);

            hron__accept_line (state, b, 0, static_cast<int> (e - b));
        }

        template<typename TTraits, typename TAlloc>
        inline void accept_line (std::basic_string<hron_char_type, TTraits, TAlloc> const & s) throw ()
        {
            assert(s.size () <= INT_MAX);

            hron__accept_line (state, s.c_str (), 0, static_cast<int> (s.size ()));
        }

    private:
        parser (parser const &);
        parser& operator= (parser const &);

        hron__parser_state state    ;

        static void document__begin (void * payload)
        {
            static_cast<i__visitor*> (payload)->document__begin ();
        }

        static void document__end (void * payload)
        {
            static_cast<i__visitor*> (payload)->document__end ();
        }

        static void preprocessor (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->preprocessor (s + b, s+ e);
        }

        static void comment (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->comment (s + b, s+ e);
        }

        static void empty (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->empty (s + b, s+ e);
        }

        static void object__begin (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->object__begin (s + b, s+ e);
        }

        static void object__end (void * payload)
        {
            static_cast<i__visitor*> (payload)->object__end ();
        }

        static void value__begin (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->value__begin (s + b, s+ e);
        }

        static void value__line (void * payload, hron_string_type s, int b, int e)
        {
            static_cast<i__visitor*> (payload)->value__line (s + b, s+ e);
        }


        static void value__end (void * payload)
        {
            static_cast<i__visitor*> (payload)->value__end ();
        }

        static void error (void* payload, int line_no, hron_string_type line, int b, int e, hron_string_type message)
        {
            static_cast<i__visitor*> (payload)->error (line_no, line + b, line + e, message);
        }


    };

    struct hron_value
    {
        typedef std::basic_string<hron_char_type>       string_type ;
        typedef std::multimap<string_type, hron_value>  values_type ;
        string_type                                     value       ;
        values_type                                     children    ;

        hron_value  () throw ()
        {
        }

        hron_value  (hron_value && hv) throw ()
            :   value   (std::move (hv.value))
            ,   children(std::move (hv.children))
        {
        }

        hron_value&  operator= (hron_value && hv) throw ()
        {
            value       = std::move (hv.value);
            children    = std::move (hv.children);

            return *this;
        }

    };

    struct hron_value__visitor : i__visitor
    {
        std::size_t             error_count ;
        hron_value              root        ;
        
        bool                    value_first ;
        hron_value::string_type value_name  ;
        hron_value::string_type value       ;

        std::stack<hron_value*> context ;

        hron_value__visitor () throw ()
            :   error_count (0)
            ,   value_first (true)

        {
        }

        virtual void    document__begin () 
        {
            context.push (std::addressof (root));
        }
        virtual void    document__end   () {}

        virtual void    preprocessor    (hron_string_type b, hron_string_type e) {}
        virtual void    comment         (hron_string_type b, hron_string_type e) {}
        virtual void    empty           (hron_string_type b, hron_string_type e) {}

        virtual void    object__begin   (hron_string_type b, hron_string_type e) 
        {
            auto obj = context.top ();
            auto iterator = obj->children.insert (hron_value::values_type::value_type (
                    hron_value::string_type (b, e)
                ,   hron_value ())
                );
            context.push (std::addressof (iterator->second));

        }
        virtual void    object__end     () 
        {
            context.pop ();
        }

        virtual void    value__begin    (hron_string_type b, hron_string_type e) 
        {
            hron_value::string_type name (b, e);
            name.swap (value_name);
            value.clear ();
            value_first = true;
        }

        virtual void    value__line     (hron_string_type b, hron_string_type e) 
        {
            if (value_first)
            {
                value_first = false;
            }
            else
            {
                value.push_back (L'\r');
                value.push_back (L'\n');
            }

            value.append (b,e);
        }

        virtual void    value__end      () 
        {
            auto obj = context.top ();
            hron_value hv;
            hv.value.swap (value);
            obj->children.insert (hron_value::values_type::value_type (
                    std::move (value_name)
                ,   std::move (hv))
                );
        }

        virtual void    error           (int line_no, hron_string_type b, hron_string_type e, hron_string_type msg) 
        { 
            ++error_count; 
        }
    };
}
// -----------------------------------------------------------------------------
