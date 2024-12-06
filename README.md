# Books API

## Database scheme 
![](https://www.plantuml.com/plantuml/png/dP2zJiD048JxUueh0SaDKXfN2LH820gK1rZR6-UK-q7hkI14ykvSuhEm8xkXykxCVkkvNNSWGNHMiOV1jtuAuV2Zj7xGfYbrPLhZaTgYk6emi5pg8qcz9xbxNAtkaqxU1n1CyBTMGcfX0ZSRR56NkaarZECB9VWASjYAdQ5yT1NB--uvV1hkojohyn3p0uJOBGSzaeqtGV_WJSG-o3AyYzQ-aLlaNyctaLh-p-_pYRi-Gtv1bPINqPeaqTKVYLDyYB2ymEQ9oOeBgq-deiW7x8Lp4YQfLF-TCSRpunXgg6RDDVicEliB)
<!--
```plantuml
@startuml
entity Book{
  * id: bigint
  --
  * title: varchar(255)
  annotation: varchar(255)
  * createdAtUtc: datetime
  artworkUrl: varchar(255)
  * numberOfCopies: number
}
entity Status{
  * id: bigint
  --
  * value: varchar(255)
}
entity Author{
  * id: bigint
  --
  * fullName: varchar(255)
}
entity Language{
  * id: bigint
  --
  * value: varchar(255)
}
entity BooksAuthors{
  * bookId: bigint
  * authorId: bigint
}
entity BooksLanguages{
  * bookId: bigint
  * languageId: bigint
}
Book ||-- Status
Book }|-- BooksAuthors
Author }|-- BooksAuthors
Book }|-- BooksLanguages
Language }|-- BooksLanguages
@enduml
```
-->