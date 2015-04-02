-----------------------------------------------------
-- A parser library in Haskell for the HRON language
--
-- Author  : Daniel Brännström
-- Date    : 2015-04-02
-- License : Microsoft Public License (Ms-PL)
-----------------------------------------------------

module HRON (parse) where

import Text.Parsec                   (modifyState)
import Text.ParserCombinators.Parsec hiding (parse)
import Control.Monad                 (void)
import Data.List                     (isSuffixOf)

-----------------------------------------------------
-- Parser type
-----------------------------------------------------

type IndentParser a = GenParser Char Int a

-----------------------------------------------------
-- HRON parse tree data structure
-----------------------------------------------------

data Preprocessor  = Preprocessor String

data ValueLine     = ContentLine String 
                   | CommentLine String String 
                   | EmptyLine String

data Member        = Value String [ValueLine] 
                   | Object String [Member] 
                   | Comment String String 
                   | Empty String

data HRON          = HRON [Preprocessor] [Member]

-----------------------------------------------------
-- Serialization of hron syntax tree which is used
-- to produce action logs
-----------------------------------------------------

instance Show Preprocessor where
    show (Preprocessor s)        = "PreProcessor:" ++ s

instance Show ValueLine where
    show (ContentLine s)         = "ContentLine:"  ++ s
    show (CommentLine e s)       = "CommentLine:"  ++ (show.length) e ++ "," ++ s 
    show (EmptyLine e)           = "EmptyLine:"    ++ e

instance Show Member where
    show (Value tag value_lines) = "Value_Begin:"  ++ tag             ++ "\n"  ++
                                   unlines (map show value_lines)              ++
                                   "Value_End:"    ++ tag
    show (Object tag members)    = "Object_Begin:" ++ tag             ++ "\n"  ++
                                   unlines (map show members)                  ++
                                   "Object_End:"   ++ tag
    show (Comment e s)           = "Comment:"      ++ (show.length) e ++ "," ++ s 
    show (Empty e)               = "Empty:"        ++ e

instance Show HRON where
    show (HRON preprocs members) = unlines (map show preprocs) ++ 
                                   unlines (map show members)

-----------------------------------------------------
-- Parser implementation
-----------------------------------------------------

indent = do
    modifyState (\i -> i + 1)
    return ()

dedent = do
    modifyState (\i -> i - 1)
    return ()

indention = do
    i <- getState
    count i (char '\t')
    return ()

eol = char '\n'

empty_string = many (oneOf " \t")

std_string = many $ noneOf "\n" 

comment_string = do
    e <- empty_string
    char '#'
    s <- std_string
    return (e,s)

preprocessor = do
    char '!'
    s <- std_string
    eol
    return $ Preprocessor s

preprocessors = many preprocessor

empty_line = do
    e <- empty_string
    eol
    return $ EmptyLine e

comment_line = do
    (e,s) <- comment_string
    eol
    return $ CommentLine e s

nonempty_line = do
    indention 
    s <- std_string
    eol
    return $ ContentLine s

value_line = 
    try(nonempty_line) <|> 
    try(comment_line)  <|> 
    try(empty_line)

value_lines = many value_line

value = do
    indention
    char '='
    tag <- std_string
    eol
    indent
    lines <- value_lines
    dedent
    return $ Value tag lines

empty = do
    e <- empty_string
    eol
    return $ Empty e

comment = do 
    (es,cs) <- comment_string
    eol
    return $ Comment es cs

object = do
    indention
    char '@'
    tag <- std_string
    eol
    indent
    ms <- members
    dedent
    return $ Object tag ms

member =  
    try(value)   <|> 
    try(object)  <|> 
    try(comment) <|>
    try(empty)

members = many member

hron = do
    ps <- preprocessors
    ms <- members
    return $ HRON ps ms

-----------------------------------------------------
-- Parser helper
-----------------------------------------------------

parse input = runParser hron 0 "" input'
    where input' = if isSuffixOf "\n" input then input else input ++ "\n"  -- kind of ugly (but simple) way to handle eol = newline | eof
