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
#include <assert.h>
#include <memory.h>
#include <stdlib.h>
// -----------------------------------------------------------------------------
#define HRON_UNUSED(p) p
// -----------------------------------------------------------------------------
struct tag__secret__parser_state
{
    int                         expected_indent    ;
    int                         indention          ;
    int                         line_no            ;
    int                         is_building_value  ;

    void *                      payload            ;

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
static void empty_void_method (void* pl)
{
    HRON_UNUSED(pl);
}
static void empty_string_method (void* pl, hron_string_type s, int begin, int end)
{
    HRON_UNUSED(pl);
    HRON_UNUSED(s);
    HRON_UNUSED(begin);
    HRON_UNUSED(end);
}

static void empty_error_method (void* pl, int line_no, hron_string_type line, int begin, int end, hron_string_type message)
{
    HRON_UNUSED(pl);
    HRON_UNUSED(line_no);
    HRON_UNUSED(line);
    HRON_UNUSED(begin);
    HRON_UNUSED(end);
    HRON_UNUSED(message);
}
// -----------------------------------------------------------------------------
void pop_context(secret__scanner_state * ss)
{
    if (ss->parser_state.is_building_value && ss->parser_state.indention < ss->parser_state.expected_indent)
    {
        --ss->parser_state.expected_indent;
        ss->parser_state.value__end(ss->parser_state.payload);
        ss->parser_state.is_building_value = 0;
    }

    while (ss->parser_state.indention < ss->parser_state.expected_indent)
    {
        --ss->parser_state.expected_indent;
        ss->parser_state.object__end(ss->parser_state.payload);
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
    HRON_UNUSED(ss);
}

static void scanner_statechoice (
        secret__scanner_state *     ss
    ,   scanner_state_choice        choice
    )
{
    assert(ss);
    switch (choice)
    {
    case SSC_From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error:
        if (ss->parser_state.is_building_value)
        {
            ss->state = ss->parser_state.expected_indent > ss->parser_state.indention
                ? SS_TagExpected
                : SS_ValueLine
                ;
        }
        else
        {
            ss->state = ss->parser_state.expected_indent < ss->parser_state.indention
                ? SS_NoContentTagExpected
                : SS_TagExpected
                ;
        }
        break;
    }
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
            ss->parser_state.value__line(ss->parser_state.payload, ss->current_line, ss->parser_state.expected_indent, ss->current_line_end);
            break;
        case SS_EndOfPreProcessorTag:
            ss->parser_state.preprocessor(ss->parser_state.payload, ss->current_line, ss->parser_state.indention + 1, ss->current_line_end);
            break;
        case SS_EndOfCommentTag:
            ss->parser_state.comment(ss->parser_state.payload, ss->current_line, ss->parser_state.indention + 1, ss->current_line_end);
            break;
        case SS_EndOfEmptyTag:
            if (ss->parser_state.is_building_value)
            {
                ss->parser_state.value__line(ss->parser_state.payload, empty, 0, 0);
            }
            else
            {
                ss->parser_state.empty(ss->parser_state.payload, ss->current_line, 0, ss->current_line_end);
            }
            break;
        case SS_EndOfObjectTag:
            pop_context(ss);
            ss->parser_state.object__begin(ss->parser_state.payload, ss->current_line, ss->parser_state.indention + 1, ss->current_line_end);
            ss->parser_state.expected_indent = ss->parser_state.indention + 1;
            break;
        case SS_EndOfValueTag:
            pop_context(ss);
            ss->parser_state.is_building_value = 1;
            ss->parser_state.value__begin(ss->parser_state.payload, ss->current_line, ss->parser_state.indention + 1, ss->current_line_end);
            ss->parser_state.expected_indent = ss->parser_state.indention + 1;
            break;
        case SS_Error:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.payload, ss->parser_state.line_no, ss->current_line, ss->current_line_begin, ss->current_line_end, "General");
            break;
        case SS_WrongTagError:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.payload, ss->parser_state.line_no, ss->current_line, ss->current_line_begin, ss->current_line_end, "WrongTag");
            break;
        case SS_NonEmptyTagError:
            ss->result = SS_Error;
            ss->parser_state.error(ss->parser_state.payload, ss->parser_state.line_no, ss->current_line, ss->current_line_begin, ss->current_line_end, "NonEmptyTag");
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

    ps.payload = visitor->payload;

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

    ss->parser_state.document__begin (ss->parser_state.payload);

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

    ss->parser_state.document__end (ss->parser_state.payload);

    free(ss);
}
// -----------------------------------------------------------------------------
void                hron__accept_line   (hron__parser_state parser_state, hron_string_type line, int begin, int end)
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

    if (begin < 0)
    {
        begin = 0;
    }
    
    if (end < begin)
    {
        end = begin;
    }

    ss->result = SR_Continue;
    scanner_accept_line (ss, line, begin, end);
}
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------

enum tag__read_line_state
{
    RLS_NewLine     ,
    RLS_Inline      ,
    RLS_ConsumedCR  ,
};

typedef enum tag__read_line_state read_line_state; 

void                hron__read_lines    (hron_string_type line, int begin, int end, read_lines_method_type visitor)
{
    int iter;
    int start;
    int count;
    read_line_state state;
    HRON_CHAR_TYPE ch;

    if (!line)
    {
        line = empty; 
    }

    if (begin < 0)
    {
        begin = 0;
    }
    
    if (end < begin)
    {
        end = begin;
    }

    if (!visitor)
    {
        return;
    }

    start = begin;
    count = 0;
    state = RLS_NewLine;

    for (iter = begin; iter < end; ++iter)
    {
        ch = line[iter];
        switch (state)
        {
            case RLS_ConsumedCR:
                visitor(line, start, count);
                switch (ch)
                {
                    case '\r':
                        start = iter;
                        count = 0;
                        state = RLS_ConsumedCR;
                        break;
                    case '\n':
                        state = RLS_NewLine;
                        break;
                    default:
                        start = iter;
                        count = 1;
                        state = RLS_Inline;
                        break;
                }
    
                break;
            case RLS_NewLine:
                start       = iter;
                count       = 0;
                switch (ch)
                {
                    case '\r':
                        state = RLS_ConsumedCR;
                        break;
                    case '\n':
                        visitor(line, start, count);
                        state = RLS_NewLine;
                        break;
                    default:
                        state = RLS_Inline;
                        ++count;
                        break;
                }
                break;
            case RLS_Inline:
            default:
                switch (ch)
                {
                    case '\r':
                        state = RLS_ConsumedCR;
                        break;
                    case '\n':
                        visitor(line, start, count);
                        state = RLS_NewLine;
                        break;
                    default:
                        ++count;
                        break;
                }
                break;
        }

        switch (state)
        {
            case RLS_NewLine:
                visitor(line, 0, 0);
                break;
            case RLS_ConsumedCR:
                visitor(line, start, count);
                visitor(line, 0, 0);
                break;
            case RLS_Inline:
            default:
                visitor(line, start, count);
                break;
        }
    }
}
// -----------------------------------------------------------------------------
