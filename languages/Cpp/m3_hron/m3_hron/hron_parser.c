// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
#include "hron_parser.h"
// -----------------------------------------------------------------------------
#include <memory.h>
#include <stdlib.h>
// -----------------------------------------------------------------------------
struct tag__secret__parser_state
{
    int                         line_no             ;
    int                         expected_indent     ;

    accept_void_method_type     document__begin    ;
    accept_void_method_type     document__end      ;

    accept_string_method_type   object__begin      ;
    accept_string_method_type   object__end        ;

    accept_string_method_type   value__begin       ;
    accept_string_method_type   value__line        ;
    accept_string_method_type   value__end         ;

    accept_error_method_type    error              ;
};
// -----------------------------------------------------------------------------
typedef struct tag__secret__parser_state* secret__parser_state; 
// -----------------------------------------------------------------------------
static void empty_void_method ()
{
}

static void empty_string_method (hron_string_type s, int begin, int end)
{
}

static void empty_error_method (int lint_no, hron_string_type message)
{
}
// -----------------------------------------------------------------------------
#define HRON_COALESCE_EMPTY_METHOD(method)  \
    if (ps->method)    ps->method= empty_void_method 

#define HRON_COALESCE_STRING_METHOD(method)  \
    if (ps->method)    ps->method= empty_string_method

#define HRON_COALESCE_ERROR_METHOD(method)  \
    if (ps->method)    ps->method= empty_error_method
// -----------------------------------------------------------------------------
hron__parser_state  hron__initialize    (hron__visitor*     visitor   )
{
    secret__parser_state    ps;

    if (!visitor)
    {
        return 0;
    }

    ps = malloc (sizeof (struct tag__secret__parser_state));
    if (!ps)
    {
        return 0;
    }

    memset(ps, 0, sizeof (struct tag__secret__parser_state));

    HRON_COALESCE_EMPTY_METHOD(document__begin);
    HRON_COALESCE_EMPTY_METHOD(document__end);

    HRON_COALESCE_STRING_METHOD(object__begin);
    HRON_COALESCE_STRING_METHOD(object__end);

    HRON_COALESCE_STRING_METHOD(value__begin);
    HRON_COALESCE_STRING_METHOD(value__end);
    HRON_COALESCE_STRING_METHOD(value__line);

    HRON_COALESCE_ERROR_METHOD(error);

    ps->document__begin ();

    return ps;
}
// -----------------------------------------------------------------------------
void                hron__finalize      (hron__parser_state parser_state)
{
    secret__parser_state    ps  = parser_state;
    if (!ps)
    {
        return;
    }

    ps->document__end ();
}
// -----------------------------------------------------------------------------
void                hron__accept_line   (hron__parser_state parser_state, hron_string_type line)
{
    int empty_string = 0;
    secret__parser_state    ps  = parser_state;
    if (!ps)
    {
        return;
    }

    if (!line)
    {
        line = (void*)&empty_string; 
    }



}
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
