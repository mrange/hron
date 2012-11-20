// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
#ifndef HRON_CHAR_TYPE
#   define HRON_CHAR_TYPE   char
#endif
// -----------------------------------------------------------------------------
typedef HRON_CHAR_TYPE      hron_char_type  ;
typedef hron_char_type*     hron_string_type;
// -----------------------------------------------------------------------------
typedef void (*accept_void_method_type)     ();

typedef void (*accept_string_method_type)   (hron_string_type, int ,int);

typedef void (*accept_error_method_type)    (int lint_no, hron_string_type message);
// -----------------------------------------------------------------------------
struct tag__hron__visitor
{
    accept_void_method_type     document__begin    ;
    accept_void_method_type     document__end      ;

    accept_string_method_type   object__begin      ;
    accept_string_method_type   object__end        ;

    accept_string_method_type   value__begin       ;
    accept_string_method_type   value__line        ;
    accept_string_method_type   value__end         ;

    accept_error_method_type    error              ;
};
typedef struct tag__hron__visitor*  hron__visitor   ;
// -----------------------------------------------------------------------------
typedef void*       hron__parser_state              ;
// -----------------------------------------------------------------------------
hron__parser_state  hron__initialize    (hron__visitor*     visitor   );
void                hron__finalize      (hron__parser_state parser_state);

void                hron__accept_line   (hron__parser_state parser_state, hron_string_type);
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
