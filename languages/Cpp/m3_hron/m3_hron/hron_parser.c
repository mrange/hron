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
#include "hron_parser.h"
// -----------------------------------------------------------------------------
#include<assert.h>
#include <memory.h>
#include <stdlib.h>
// -----------------------------------------------------------------------------
struct tag__secret__parser_state
{
    int                         expected_indent    ;
    int                         indention          ;
    int                         line_no            ;
    int                         is_building_value  ;

    accept_void_method_type     document__begin    ;
    accept_void_method_type     document__end      ;

    accept_string_method_type   preprocessor       ;
    accept_string_method_type   comment            ;
    accept_string_method_type   empty              ;

    accept_string_method_type   object__begin      ;
    accept_void_method_type     object__end        ;

    accept_string_method_type   value__begin       ;
    accept_string_method_type   value__line        ;
    accept_void_method_type     value__end         ;

    accept_error_method_type    error              ;
};
// -----------------------------------------------------------------------------
typedef struct tag__secret__parser_state secret__parser_state; 
// -----------------------------------------------------------------------------
#include "../../../../tools/M3.HRON.Generator/M3.HRON.Generator/Generated_CParser.c"
// -----------------------------------------------------------------------------
static void empty_void_method ()
{
}
static void empty_string_method (hron_string_type s, int begin, int end)
{
}

static void empty_error_method (int lint_no, hron_string_type line, hron_string_type message)
{
}
// -----------------------------------------------------------------------------
void pop_context(secret__scanner_state * ss)
{
    if (ss->parser_state.is_building_value && ss->parser_state.indention < ss->parser_state.expected_indent)
    {
        --ss->parser_state.expected_indent;
        ss->parser_state.value__end();
        ss->parser_state.is_building_value = 0;
    }

    while (ss->parser_state.indention < ss->parser_state.expected_indent)
    {
        ss->parser_state.expected_indent;
        ss->parser_state.object__end();
    }

}

static void scanner_begin_line (secret__scanner_state * ss)
{
    switch (ss->state)
    {
    case SS_PreProcessing:
    case SS_PreProcessorTag:
    case SS_EndOfPreProcessorTag:
        ss->state = SS_PreProcessing;
        break;
    default:
        ss->state = SS_Indention;
        break;
    }

    ss->parser_state.indention = 0;
    ++ss->parser_state.line_no;
}

static void scanner_end_line (secret__scanner_state * ss)
{
}

static void scanner_statechoice (
        secret__scanner_state *     ss
    ,   scanner_state_choice        choice
    )
{
    assert(ss);
}

hron_string_type empty = "";

static void scanner_statetransition (
        secret__scanner_state *     ss
    ,   scanner_state               from
    ,   scanner_state               to
    ,   scanner_state_transition    sst
    )
{
    assert(ss);
    switch (sst)
    {
    case SST_From_Indention__To_Indention:
        ++ss->parser_state.indention;
        break;
    }

    switch (to)
    {
        case SS_PreProcessorTag:
        case SS_EmptyTag:
        case SS_CommentTag:
        case SS_ValueTag:
        case SS_ValueLine:
        case SS_ObjectTag:
            ss->result = SR_Done;
            break;
        case SS_EndOfValueLine:
            ss->parser_state.value__line(ss->current_line, ss->parser_state.expected_indent, INT_MAX);
            break;
        case SS_EndOfPreProcessorTag:
            ss->parser_state.preprocessor(ss->current_line, ss->parser_state.indention + 1, INT_MAX);
            break;
        case SS_EndOfCommentTag:
            ss->parser_state.preprocessor(ss->current_line, ss->parser_state.indention + 1, INT_MAX);
            break;
        case SS_EndOfEmptyTag:
            if (ss->parser_state.is_building_value)
            {
                ss->parser_state.value__line(empty, 0, 0);
            }
            else
            {
                ss->parser_state.empty(ss->current_line, 0, INT_MAX);
            }
            break;
        case SS_EndOfObjectTag:
            pop_context(ss);
            ss->parser_state.object__begin(ss->current_line, ss->parser_state.indention + 1, INT_MAX);
            ss->parser_state.expected_indent = ss->parser_state.indention + 1;
            break;
        case SS_EndOfValueTag:
            pop_context(ss);
            ss->parser_state.is_building_value = 1;
            ss->parser_state.value__begin(ss->current_line, ss->parser_state.indention + 1, INT_MAX);
            ss->parser_state.expected_indent = ss->parser_state.indention + 1;
            break;
        case SS_Error:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.line_no, ss->current_line, "General");
            break;
        case SS_WrongTagError:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.line_no, ss->current_line, "WrongTag");
            break;
        case SS_NonEmptyTagError:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.line_no, ss->current_line, "NonEmptyTag");
            break;
    }


}
// -----------------------------------------------------------------------------
#define HRON_COALESCE_EMPTY_METHOD(method)  \
    ps.method   = visitor->method ? visitor->method : empty_void_method 

#define HRON_COALESCE_STRING_METHOD(method)  \
    ps.method   = visitor->method ? visitor->method : empty_string_method 

#define HRON_COALESCE_ERROR_METHOD(method)  \
    ps.method   = visitor->method ? visitor->method : empty_error_method 

// -----------------------------------------------------------------------------
hron__parser_state  hron__initialize    (hron__visitor*     visitor   )
{
    secret__parser_state    ps;
    secret__scanner_state * ss;

    if (!visitor)
    {
        return 0;
    }

    memset(&ps, 0, sizeof (struct tag__secret__parser_state));

    HRON_COALESCE_EMPTY_METHOD(document__begin);
    HRON_COALESCE_EMPTY_METHOD(document__end);

    HRON_COALESCE_STRING_METHOD(preprocessor);
    HRON_COALESCE_STRING_METHOD(comment);
    HRON_COALESCE_STRING_METHOD(empty);

    HRON_COALESCE_STRING_METHOD(object__begin);
    HRON_COALESCE_EMPTY_METHOD(object__end);

    HRON_COALESCE_STRING_METHOD(value__begin);
    HRON_COALESCE_EMPTY_METHOD(value__end);
    HRON_COALESCE_STRING_METHOD(value__line);

    HRON_COALESCE_ERROR_METHOD(error);

    ss = malloc (sizeof(secret__scanner_state));
    if (!ss)
    {
        return 0;
    }

    scanner_init (ss, SS_PreProcessorTag, &ps);

    ss->parser_state.document__begin ();

    return ss;
}
// -----------------------------------------------------------------------------
void                hron__finalize      (hron__parser_state parser_state)
{
    secret__scanner_state * ss  = parser_state;
    if (!ss)
    {
        return;
    }

    ss->parser_state.indention = 0;
    pop_context (ss);

    ss->parser_state.document__end ();

    free(ss);
}
// -----------------------------------------------------------------------------
void                hron__accept_line   (hron__parser_state parser_state, hron_string_type line)
{
    secret__scanner_state * ss  = parser_state;
    if (!ss)
    {
        return;
    }

    if (!line)
    {
        line = empty; 
    }

    scanner_accept_line (ss, line);
}
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
