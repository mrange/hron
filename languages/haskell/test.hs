-----------------------------------------------------
-- Unit tests for module hron.hs
--
-- Author  : Daniel Brännström
-- Date    : 2015-04-02
-- License : Microsoft Public License (Ms-PL)
-----------------------------------------------------

import Test.HUnit
import HRON
import System.IO  
import Control.Monad

mkTest id = TestCase $
	do
		hronFile <- openFile ("../../reference-data/" ++ id ++ ".hron") ReadMode
		hSetEncoding hronFile utf8_bom
		hron <- hGetContents hronFile
		
		hronActionLogFile <- openFile ("../../reference-data/" ++ id ++ ".hron.actionlog") ReadMode
		hSetEncoding hronActionLogFile utf8_bom
		hronActionLog <- hGetContents hronActionLogFile

		case parse hron of
			Left error -> 
				assertFailure (show error)
			Right tree -> do
				let log = show tree 

				logFile <- openFile ("test-" ++ id ++ ".hron.actionlog") WriteMode
				hSetEncoding logFile utf8_bom
				hPutStr logFile log
				hClose logFile

				assertEqual "action logs should be equal" hronActionLog log

		hClose hronFile
		hClose hronActionLogFile


tests = TestList [
	TestLabel "hello world" (mkTest "helloworld"),
	TestLabel "simple" (mkTest "simple"),
	TestLabel "random" (mkTest "random"),
	TestLabel "large" (mkTest "large")
	]

runTests = runTestTT tests
rt = runTests
