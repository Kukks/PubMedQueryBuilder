Builds PubMed search strings for the most common scenarios using an easily constructable file

File format 
```
---data-set--- set-operator=OR
abc
def
ghi
---data-set--- set-operator=OR prev-operator=AND
jkl
mno
---data-set--- set-operator=OR prev-operator=AND
pqrs[customcategory]
```

which results in the search string
```
((("abc"[Title/Abstract]) OR ("def"[Title/Abstract]) OR ("ghi"[Title/Abstract])) AND (("jkl"[Title/Abstract]) OR ("mno"[Title/Abstract])) AND (("pqrs"[customcategory])))
```

Uses .NET Core C# and works across linux, windows and mac, if you know how to compile and run. :)

