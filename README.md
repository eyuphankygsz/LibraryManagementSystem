# LibraryManagementSystem

Simple Library Management System written in C#!

## Book and Borrow Sample Included!
If you don't want to add books one by one from scratch, I created 21 books and 6 book borrows.
Please extract them to **"My Documents" or "Documents"** folder

## About Book Class

I created Book class to store the books into objects. Each Book object has different isbn.

**Book class include 5 variables:**\
   -bName (Book Name),\
   -author (Author),\
   -isbn (ISBN),\
   -copy (Current copy count of the book in the library)\
   -borrow (Current borrowed count of the book)\
except "copy" and "borrow", everything is readonly because they will never change.

**8 Methods:**\
   -5 of them is for get variables,\
   -SetInfo(bool increase) used for increasing and decreasing copy and borrow.
   -1 Set Copy and 1 Set Borrow

**1 Custom Constructor**

## About Borrowed Class

I created Borrowed Class to store borrowed list. Each Borrowed object has unique Borrow Code.
You can use this Borrow Code to return the book.

**Borrowed Class include 4 variables:**\
   -isbn (ISBN of the borrowed book),\
   -borrowCode (To return and check the unreturned book.),\
   -borrowedDate (To get did this book borrowed),\
   -returnDate (To get the deadline of the book to returned)

**4 Methods:**\
   -All of them is for get variables

**1 Custom Constructor**

## Adding a new book
1. Select the Add a new book by pressing 1 in the main menu.
2. Enter the Name of the book,
3. Enter the Author of the book,
4. Enter the ISBN of the book
5. And enter the copiy count this library have.

## Borrow a book
1. Select Borrow a book by pressing 6 in the main menu.
2. Search the book you want,
3. **Don't forget that you can change pages by pressing 8 and 9.**
4. Select the right book,
5. Check the selected book and confirm the selection,
6. Check the latest return date and **COPY YOUR BORROW CODE** to return the book later!

## Edit an existing book

You can either set copy count, set borrow count or delete the book entirely!
1. Select Edit a book by pressing 8 in the main menu.
2. Search the book you want,
3. Select the right book to edit,
4. Select one of the 4 options,

### How I create the Borrow Code?

```http
  string borrowCode = books.IndexOf(selected).ToString() + selected.GetCopy() + rand.Next(100,1000) + borrowedList.Count;
```
|  Book ID  |  Book's Current Copy Count | Random(100-1000) | Borrow ID |
| :-------- | :------------------------- | :--------------- | :-------- |
|    `12`   |            `21`            |       `253`      |    `0`    |

```http
 borrowCode = 12212530
```
|  Book ID  |  Book's Current Copy Count | Random(100-1000) | Borrow ID |
| :-------- | :------------------------- | :--------------- | :-------- |
|    `12`   |            `20`            |       `253`      |    `1`    |

```http
 borrowCode = 12202531
```
by this way, Borrow Code always be unique.
## Why did I use "byte" instead of "int"?
This program is for a library. From what I've seen, there aren't many copies of the same book in libraries.
To reduce the usage of memory space, I prefer to use byte.

Yes, I know, for a modern computers this might be nothing but if you have thousands and thousands of different books, that might be a problem.
