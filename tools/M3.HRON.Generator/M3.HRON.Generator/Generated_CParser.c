

// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################







enum tag__scanner_state
{
    SS_Error,
    SS_WrongTagError,
    SS_NonEmptyTagError,
    SS_PreProcessing,
    SS_Indention,
    SS_TagExpected,
    SS_NoContentTagExpected,
    SS_PreProcessorTag,
    SS_ObjectTag,
    SS_ValueTag,
    SS_EmptyTag,
    SS_CommentTag,
    SS_EndOfPreProcessorTag,
    SS_EndOfObjectTag,
    SS_EndOfEmptyTag,
    SS_EndOfValueTag,
    SS_EndOfCommentTag,
    SS_ValueLine,
    SS_EndOfValueLine,
};

enum tag__scanner_state_transition
{
    SST_From_Error__To_Error,
    SST_From_WrongTagError__To_Error,
    SST_From_NonEmptyTagError__To_Error,
    SST_From_PreProcessing__To_PreProcessorTag,
    SST_From_PreProcessing__To_Indention,
    SST_From_Indention__To_EndOfEmptyTag,
    SST_From_Indention__To_Indention,
    SST_From_Indention__To_TagExpected,
    SST_From_Indention__To_NoContentTagExpected,
    SST_From_Indention__To_ValueLine,
    SST_From_Indention__To_Error,
    SST_From_TagExpected__To_EndOfEmptyTag,
    SST_From_TagExpected__To_ObjectTag,
    SST_From_TagExpected__To_ValueTag,
    SST_From_TagExpected__To_CommentTag,
    SST_From_TagExpected__To_EmptyTag,
    SST_From_TagExpected__To_WrongTagError,
    SST_From_NoContentTagExpected__To_EndOfEmptyTag,
    SST_From_NoContentTagExpected__To_CommentTag,
    SST_From_NoContentTagExpected__To_EmptyTag,
    SST_From_NoContentTagExpected__To_WrongTagError,
    SST_From_PreProcessorTag__To_EndOfPreProcessorTag,
    SST_From_PreProcessorTag__To_PreProcessorTag,
    SST_From_ObjectTag__To_EndOfObjectTag,
    SST_From_ObjectTag__To_ObjectTag,
    SST_From_ValueTag__To_EndOfValueTag,
    SST_From_ValueTag__To_ValueTag,
    SST_From_EmptyTag__To_EndOfEmptyTag,
    SST_From_EmptyTag__To_EmptyTag,
    SST_From_EmptyTag__To_NonEmptyTagError,
    SST_From_CommentTag__To_EndOfCommentTag,
    SST_From_CommentTag__To_CommentTag,
    SST_From_EndOfPreProcessorTag__To_PreProcessing,
    SST_From_EndOfObjectTag__To_Indention,
    SST_From_EndOfEmptyTag__To_Indention,
    SST_From_EndOfValueTag__To_Indention,
    SST_From_EndOfCommentTag__To_Indention,
    SST_From_ValueLine__To_EndOfValueLine,
    SST_From_ValueLine__To_ValueLine,
    SST_From_EndOfValueLine__To_Indention,
};

enum tag__scanner_state_choice
{
    SSC_From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error,
};

enum tag__scanner_result
{
    SR_Error   ,
    SR_Continue,
    SR_Done    ,
};

typedef enum    tag__scanner_state              scanner_state           ;    
typedef enum    tag__scanner_state_transition   scanner_state_transition;    
typedef enum    tag__scanner_state_choice       scanner_state_choice    ;    
typedef enum    tag__scanner_result             scanner_result          ;    

struct tag__secret__scanner_state
{
    scanner_result          result          ;
    scanner_state           state           ;
    hron_string_type        current_line    ;
    hron_char_type          current_char    ;
    secret__parser_state    parser_state    ;
};

typedef struct  tag__secret__scanner_state      secret__scanner_state   ;

static void scanner_begin_line (secret__scanner_state * ss);

static void scanner_end_line (secret__scanner_state * ss);

static void scanner_statechoice (
        secret__scanner_state *     ss
    ,   scanner_state_choice        choice
    );

static void scanner_statetransition (
        secret__scanner_state *     ss
    ,   scanner_state               from
    ,   scanner_state               to
    ,   scanner_state_transition    sst
    );

static void scanner_init (
        secret__scanner_state * ss
    ,   scanner_state           initial_state
    ,   secret__parser_state *  ps
    )
{
    assert (ss);
    assert (ps);

    memset (ss, 0, sizeof (secret__scanner_state));
    ss->result          = SR_Continue   ;
    ss->state           = initial_state ;
    ss->parser_state    = *ps           ;
}

scanner_result scanner_accept_line (secret__scanner_state * ss, hron_string_type hs)
{
    assert(ss);
    assert(hs);

    ss->current_line = hs;
    ss->current_char = *(ss->current_line);
    
    scanner_begin_line (ss);

    if (!ss->current_char)
    {
        goto end;
    }

    do 
    {
apply:
        if (ss->result != SR_Continue)
        {
            break;
        } 
        
        switch (ss->state)
        {
        case SS_Error:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_Error
                                ,   SS_Error
                                ,   SST_From_Error__To_Error
                                );
                    break;
    
                }
            break;
        case SS_WrongTagError:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Error; 
                            scanner_statetransition (
                                    ss
                                ,   SS_WrongTagError
                                ,   SS_Error
                                ,   SST_From_WrongTagError__To_Error
                                );
                    break;
    
                }
            break;
        case SS_NonEmptyTagError:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Error; 
                            scanner_statetransition (
                                    ss
                                ,   SS_NonEmptyTagError
                                ,   SS_Error
                                ,   SST_From_NonEmptyTagError__To_Error
                                );
                    break;
    
                }
            break;
        case SS_PreProcessing:
            switch (ss->current_char)
            {
            case '!':
                    ss->state = SS_PreProcessorTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessing
                                ,   SS_PreProcessorTag
                                ,   SST_From_PreProcessing__To_PreProcessorTag
                                );
                    break;
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessing
                                ,   SS_Indention
                                ,   SST_From_PreProcessing__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_Indention:
            switch (ss->current_char)
            {
            case '\t':
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_Indention
                                ,   SST_From_Indention__To_Indention
                                );
                    break;
            default:
                    scanner_statechoice (
                            ss
                        ,   SSC_From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error
                        );

                switch (ss->state)
                {
                case SS_TagExpected:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_TagExpected
                                ,   SST_From_Indention__To_TagExpected
                                );
                    break;
                case SS_NoContentTagExpected:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_NoContentTagExpected
                                ,   SST_From_Indention__To_NoContentTagExpected
                                );
                    break;
                case SS_ValueLine:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_ValueLine
                                ,   SST_From_Indention__To_ValueLine
                                );
                    break;
                case SS_Error:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_Error
                                ,   SST_From_Indention__To_Error
                                );
                    break;
                    default:
                        ss->result = SR_Error;
                        break;
                    }
                    goto apply;
                    break;
    
                }
            break;
        case SS_TagExpected:
            switch (ss->current_char)
            {
            case '@':
                    ss->state = SS_ObjectTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_ObjectTag
                                ,   SST_From_TagExpected__To_ObjectTag
                                );
                    break;
            case '=':
                    ss->state = SS_ValueTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_ValueTag
                                ,   SST_From_TagExpected__To_ValueTag
                                );
                    break;
            case '#':
                    ss->state = SS_CommentTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_CommentTag
                                ,   SST_From_TagExpected__To_CommentTag
                                );
                    break;
            case '\t':
            case ' ':
                    ss->state = SS_EmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_EmptyTag
                                ,   SST_From_TagExpected__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_WrongTagError; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_WrongTagError
                                ,   SST_From_TagExpected__To_WrongTagError
                                );
                    break;
    
                }
            break;
        case SS_NoContentTagExpected:
            switch (ss->current_char)
            {
            case '#':
                    ss->state = SS_CommentTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_CommentTag
                                ,   SST_From_NoContentTagExpected__To_CommentTag
                                );
                    break;
            case '\t':
            case ' ':
                    ss->state = SS_EmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_EmptyTag
                                ,   SST_From_NoContentTagExpected__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_WrongTagError; 
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_WrongTagError
                                ,   SST_From_NoContentTagExpected__To_WrongTagError
                                );
                    break;
    
                }
            break;
        case SS_PreProcessorTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessorTag
                                ,   SS_PreProcessorTag
                                ,   SST_From_PreProcessorTag__To_PreProcessorTag
                                );
                    break;
    
                }
            break;
        case SS_ObjectTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ObjectTag
                                ,   SS_ObjectTag
                                ,   SST_From_ObjectTag__To_ObjectTag
                                );
                    break;
    
                }
            break;
        case SS_ValueTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueTag
                                ,   SS_ValueTag
                                ,   SST_From_ValueTag__To_ValueTag
                                );
                    break;
    
                }
            break;
        case SS_EmptyTag:
            switch (ss->current_char)
            {
            case '\t':
            case ' ':
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_EmptyTag
                                ,   SST_From_EmptyTag__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_NonEmptyTagError; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_NonEmptyTagError
                                ,   SST_From_EmptyTag__To_NonEmptyTagError
                                );
                    break;
    
                }
            break;
        case SS_CommentTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_CommentTag
                                ,   SS_CommentTag
                                ,   SST_From_CommentTag__To_CommentTag
                                );
                    break;
    
                }
            break;
        case SS_EndOfPreProcessorTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_PreProcessing; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfPreProcessorTag
                                ,   SS_PreProcessing
                                ,   SST_From_EndOfPreProcessorTag__To_PreProcessing
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_EndOfObjectTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfObjectTag
                                ,   SS_Indention
                                ,   SST_From_EndOfObjectTag__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_EndOfEmptyTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfEmptyTag
                                ,   SS_Indention
                                ,   SST_From_EndOfEmptyTag__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_EndOfValueTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfValueTag
                                ,   SS_Indention
                                ,   SST_From_EndOfValueTag__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_EndOfCommentTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfCommentTag
                                ,   SS_Indention
                                ,   SST_From_EndOfCommentTag__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        case SS_ValueLine:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueLine
                                ,   SS_ValueLine
                                ,   SST_From_ValueLine__To_ValueLine
                                );
                    break;
    
                }
            break;
        case SS_EndOfValueLine:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfValueLine
                                ,   SS_Indention
                                ,   SST_From_EndOfValueLine__To_Indention
                                );
                    goto apply;
                    break;
    
                }
            break;
        default:
            ss->result = SR_Error;
            break;
        }        
    }
    while (ss->current_char = *(++ss->current_line));

    if (ss->result == SR_Error)
    {
        goto end;
    } 
        
    switch (ss->state)
    {
    case SS_Indention:
            ss->state = SS_EndOfEmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_Indention__To_EndOfEmptyTag
                                );
        break;
    case SS_TagExpected:
            ss->state = SS_EndOfEmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_TagExpected__To_EndOfEmptyTag
                                );
        break;
    case SS_NoContentTagExpected:
            ss->state = SS_EndOfEmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_NoContentTagExpected__To_EndOfEmptyTag
                                );
        break;
    case SS_PreProcessorTag:
            ss->state = SS_EndOfPreProcessorTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessorTag
                                ,   SS_EndOfPreProcessorTag
                                ,   SST_From_PreProcessorTag__To_EndOfPreProcessorTag
                                );
        break;
    case SS_ObjectTag:
            ss->state = SS_EndOfObjectTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_ObjectTag
                                ,   SS_EndOfObjectTag
                                ,   SST_From_ObjectTag__To_EndOfObjectTag
                                );
        break;
    case SS_ValueTag:
            ss->state = SS_EndOfValueTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueTag
                                ,   SS_EndOfValueTag
                                ,   SST_From_ValueTag__To_EndOfValueTag
                                );
        break;
    case SS_EmptyTag:
            ss->state = SS_EndOfEmptyTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_EmptyTag__To_EndOfEmptyTag
                                );
        break;
    case SS_CommentTag:
            ss->state = SS_EndOfCommentTag; 
                            scanner_statetransition (
                                    ss
                                ,   SS_CommentTag
                                ,   SS_EndOfCommentTag
                                ,   SST_From_CommentTag__To_EndOfCommentTag
                                );
        break;
    case SS_ValueLine:
            ss->state = SS_EndOfValueLine; 
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueLine
                                ,   SS_EndOfValueLine
                                ,   SST_From_ValueLine__To_EndOfValueLine
                                );
        break;
    }

end:
    scanner_end_line (ss);

    return ss->result;
}





