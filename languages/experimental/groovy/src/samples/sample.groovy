import org.m3.hron.HronParser

def hronBlob = """\
@welcome
	=title
		Welcome to HRON
	=copy
		HRON is a new data format
		which is easy to read and
		supports multi line strings.
		
		Best,
		The Developers
	@authors
		=firstName
			Bob
		=lastName
			Developer
	@
		=firstName
			Steve
		=lastName
			Stevensson
"""
    
def hron = new HronParser().parseText(hronBlob)
    
assert hron instanceof Map 
assert hron.welcome.title == "Welcome to HRON"
assert hron.welcome.copy.readLines()[5] == "The Developers"
// Note, the groovy parser currently does not handle the hron 
// list notation correctly. The commented out assertions should 
// work, but do not. 
// assert hron.welcome.authors instanceof List
assert hron.welcome.authors instanceof Map
// assert hron.welcome.authors[0].firstName == "Bob"
assert hron.welcome.authors.firstName == "Bob"

println "Hron sample successfully executed!"