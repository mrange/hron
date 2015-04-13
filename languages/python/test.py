import unittest
import hron

class RegexCacheTest(unittest.TestCase):
    def test_CacheHit(self):
        cache = hron._RegexCache(lambda i: "\d{" + str(i) + "}")
        re = cache.get(4)
        self.assertEqual(re.pattern, "\d{4}")
        self.assertTrue(re.match("1234"))

    def test_CacheMiss(self):
        cache = hron._RegexCache(lambda i: "\d{" + str(i) + "}")
        re = cache.get(14)
        self.assertEqual(re.pattern, "\d{14}")
        self.assertTrue(re.match("12345678901234"))

class DeserializationTests(unittest.TestCase):
    def _getTestData(self, fileid):
        with open('..\\..\\reference-data\\' + fileid, 'r', encoding='utf-8-sig') as f:
            return f.readlines()

    def _run(self, tid):
        text = self._getTestData(tid)
        ctx = hron._DeserializationState(text)
        ctx.enableLogging()
        o = hron.parse(ctx)
        self.assertIsNotNone(o)
        log = ctx.actionLog
        logRef = list(map(lambda s: str.rstrip(s, "\r\n"), self._getTestData(tid + ".actionlog")))
        self.assertEqual(log, logRef)

    def test_Deserialize_HelloWorld(self):
        self._run("helloworld.hron")

    def test_Deserialize_Simple(self):
        self._run("simple.hron")

    def test_Deserialize_Random(self):
        self._run("random.hron")

    def test_Deserialize_Large(self):
        self._run("large.hron")

class SerializationTests(unittest.TestCase):
    def test_Basic(self):
        o = hron._Dynamic()
        o.logPath = "Logs\\CurrentDay"
        o.welcomeMessage = "Hello there"
        o.names = ["Daniel", "Sven", "Kalle"]
        o.persons = { 'Tony': 10, 'August': 30 }
        o2 = hron._Dynamic()
        o2.prop1 = "Again\nThere is more to it"
        o2.prop2 = "Even more"
        o.complex = o2;
        hronString = hron.serialize(o)
        expected = """\
@complex
	=prop1
		Again
		There is more to it
	=prop2
		Even more
=logPath
	Logs\CurrentDay
=names
	Daniel
=names
	Sven
=names
	Kalle
@persons
	=August
		30
	=Tony
		10
=welcomeMessage
	Hello there"""
        self.assertEqual(hronString, expected)

if __name__ == '__main__':
    unittest.main()
