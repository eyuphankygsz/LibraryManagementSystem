using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KutuphaneYonetimSistemi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LibrarySystem sys = new LibrarySystem();
            sys.Initialize();
        }


    }

    class LibrarySystem
    {
        Library? library;
        public void Initialize()
        {
            library = new Library(this);
            library.GetBooks();
            MainMenu();
        }

        void MainMenu()
        {
            char choose = ' ';
            while (true)
            {
                DisplayMainMenu();
                if (choose == 'p')
                    Console.WriteLine("You selected an option that does not exist! Please enter new input...");

                if (!GetChoose('1', '6', ref choose, "Menu"))
                    continue;

                MainMenuRouter(choose);
            }
        }
        void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("*************************************");
            Console.WriteLine("| Welcome to the library system V0.1|");
            Console.WriteLine("*************************************");

            Console.WriteLine("1-) Add a new book.");
            Console.WriteLine("2-) Display all books.");
            Console.WriteLine("3-) Search a book.");
            Console.WriteLine("4-) Borrow a book.");
            Console.WriteLine("5-) Return a book.");
            Console.WriteLine("6-) Display expired books.");
            Console.WriteLine("X-) Exit the program.");
        }
        public bool GetChoose(char min, char max, ref char choose, string from)
        {
            choose = Console.ReadKey().KeyChar;
            if (choose >= min && choose <= max)
                return true;
            else
            if (choose == 'x' || choose == 'X')
            {
                if (from == "Menu")
                {
                    Console.Clear();
                    Console.WriteLine("Good bye!");
                    Environment.Exit(0);
                }
            }
            if (from == "Display")
            {
                if (choose == '8' || choose == '9' || choose == 'x')
                {
                    return true;
                }
            }

            choose = 'p';
            return false;
        }

        void MainMenuRouter(char input)
        {
            Console.Clear();
            switch (input)
            {
                case '1':
                    library?.AddBook();
                    break;
                case '2':
                    library?.DisplayBooks(unselectable: true, 0, '0', library.ListOfBooks());
                    break;
                case '3':
                    library?.SearchBook(display: true);
                    break;
                case '4':
                    library?.BorrowBook();
                    break;
                case '5':
                    library?.ReturnBook();
                    break;
                case '6':
                    library?.UnreturnedBooks();
                    break;
            }
        }

    }

    class Book
    {
        string bName, author, isbn;
        byte copy, borrow;

        public string GetName() => bName;
        public string GetAuthor() => author;
        public string GetIsbn() => isbn;
        public byte GetCopy() => copy;
        public byte GetBorrow() => borrow;

        public void SetInfo(bool increase)
        {
            if (increase)
            {
                copy++;
                borrow--;
            }
            else
            {
                copy--;
                borrow++;
            }
        }
        public void SetCopy(byte copy) => this.copy = copy;
        public void SetBorrow(byte borrow) => this.borrow = borrow;

        public Book(string bName, string author, string isbn, byte copy, byte borrow)
        {
            this.bName = bName;
            this.author = author;
            this.isbn = isbn;
            this.copy = copy;
            this.borrow = borrow;
        }
    }

    class Borrowed
    {
        string isbn;
        string borrowCode;
        DateTime borrowedDate;
        DateTime returnDate;

        public string ISBN() => isbn;
        public string BorrowCode() => borrowCode;
        public DateTime B_Date() => borrowedDate;
        public DateTime R_Date() => returnDate;

        public Borrowed(string isbn, string borrowCode, DateTime borrowedDate, DateTime returnDate)
        {
            this.isbn = isbn;
            this.borrowCode = borrowCode;
            this.borrowedDate = borrowedDate;
            this.returnDate = returnDate;
        }
    }

    class Library
    {
        List<Book> books = new List<Book>();
        List<Borrowed> borrowedList = new List<Borrowed>();
        string bookPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "books.txt");
        string borrowPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "borrowed.txt");
        public List<Book> ListOfBooks()
        {
            return books;
        }
        LibrarySystem libSystem;
        public Library(LibrarySystem libSystem)
        {
            this.libSystem = libSystem;
        }
        public void AddBook()
        {
            char choose = ' ';

            do
            {
                Console.Clear();
                string bName = SetStrings("Please enter the book's name (Ex. Red Dead): ", " ");
                if (bName.Length == 0) break;
                string author = SetStrings("Please enter the book's author (Ex. Arthur Morgan): ", " ");
                if (author.Length == 0) break;
                string isbn = SetStrings("Please enter the book's ISBN (Ex. 9876543210987): ", "isbn");
                if (isbn.Length == 0) break;
                byte copy = SetBytes("Please enter how many copies of this book this library have: ", 1);
                byte borrow = SetBytes("Please enter how many copies of this book has been borrowed: ", 0);
                Console.WriteLine("Adding new book to the list...");

                Book newBook = new Book(bName, author, isbn, copy, borrow);
                books.Add(newBook);
                WriteBook(newBook);
                Console.Clear();
                Console.WriteLine("All done!");
                Console.WriteLine("1-) Add new book");
                Console.WriteLine("Or press any key to return to main menu...");
                choose = Console.ReadKey().KeyChar;
            } while (choose == '1');

        }
        byte SetBytes(string message, byte min)
        {
            Console.Clear();
            bool error = false;
            while (true)
            {
                if (error)
                    Console.WriteLine("Please enter numeric value. ({0} to 255)", min);
                error = true;

                Console.WriteLine(message);
                string input = Console.ReadLine();

                if (byte.TryParse(input, out byte selected))
                {
                    if (selected < min)
                        continue;
                    else
                        return Convert.ToByte(input);
                }

            }
        }
        string SetStrings(string message, string from)
        {
            Console.Clear();
            bool error = false;
            string errorMessage = "";
            while (true)
            {

                Console.WriteLine("(To cancel adding a new book, press ENTER without write anything.)");
                if (error)
                    Console.WriteLine(errorMessage);
                errorMessage = "Please enter at least 3 characters.";
                error = true;
                Console.Write(message);
                string str = Console.ReadLine();
                if (from.Equals("isbn"))
                {
                    errorMessage = "The ISBN code must consist of numbers only, be at least 3 digits and unique.";
                    if (!ISBNCheck(str))
                        continue;
                }
                if (str.Length == 0)
                {
                    return str;
                }
                if (str.Length >= 3)
                    return str;
            }
        }
        bool ISBNCheck(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                    return false;
            }
            bool match = false;
            books.ForEach(delegate (Book b)
            {
                if (b.GetIsbn().Equals(str))
                {
                    match = true;
                }
            }
            );
            if (match)
                return false;

            return true;
        }


        public Book DisplayBooks(bool unselectable, int offSet, char choose, List<Book> found)
        {
            if (found.Count == 0)
            {
                Console.WriteLine("No Books Found!\nPress any key to return to back...");
                Console.ReadKey();
                return null;
            }

            Book book;
            List<Book> page = found.GetRange(offSet, (int)MathF.Min(found.Count, offSet + 5));
            do
            {
                Console.Write("\u001b[2J\u001b[3J");
                Console.Clear();
                int counter = 1;
                page.ForEach(x =>
                Console.WriteLine("{0}-) Name: {1}\nAuthor: {2}\nISBN: {3}\nCopies: {4}\nBorrowed: {5}\n-----------------------\n",
                        (unselectable) ? offSet + counter++ : counter++,
                         x.GetName(),
                          x.GetAuthor(),
                           x.GetIsbn(),
                            x.GetCopy(),
                             x.GetBorrow()));

                if (offSet != 0)
                    Console.WriteLine("8-) Previous Page");
                if (offSet + page.Count < found.Count)
                    Console.WriteLine("9-) Next Page");

                Console.WriteLine("x-) Return to main menu.");
                if (!unselectable)
                {
                    Console.Write("Select the one you want to borrow or return: ");
                    if (!libSystem.GetChoose('1', Convert.ToChar((char)MathF.Min(page.Count + '0', '5')), ref choose, "Display"))
                        continue;

                }
                else
                {
                    if (!libSystem.GetChoose('8', '9', ref choose, "Display"))
                        continue;
                }


                page = PageChange(ref offSet, choose, found);

                if (choose == 'x')
                {
                    return null;
                }

            } while (choose == 'p' || choose == '8' || choose == '9');

            if (!unselectable)
            {
                return page[choose - '0' - 1];
            }
            return null;

        }

        public List<Book> SearchBook(bool display)
        {
            char choose = ' ';
            List<Book> found = new List<Book>();
            do
            {
                Console.Clear();
                Console.WriteLine("Please choose which of the following you would like to search with");
                Console.WriteLine("1-)By book name.");
                Console.WriteLine("2-)By author name.");

                if (choose == 'p')
                    Console.WriteLine("You selected an option that does not exist! Please enter new input...");

                if (!libSystem.GetChoose('1', '2', ref choose, "Search"))
                    continue;

                Console.Clear();
                switch (choose)
                {
                    case '1':
                        Console.WriteLine("Please enter the book name: ");
                        found = SearchBy(Console.ReadLine(), "bName");
                        break;
                    case '2':
                        Console.WriteLine("Please enter the author's name");
                        found = SearchBy(Console.ReadLine(), "author");
                        break;
                }

                if (display)
                {
                    DisplayBooks(unselectable: true, 0, choose, found);
                    Console.Clear();
                    Console.WriteLine("1-)Search another book.");
                    Console.WriteLine("Or press any key to go back...");
                    choose = Console.ReadKey().KeyChar;
                }
                else
                    choose = '2';
            } while (choose == '1' || choose == 'p');
            return found;
        }
        List<Book> SearchBy(string name, string from)
        {
            if (name.Length == 0)
            {
                Console.WriteLine("You entered nothing, you are routing to main menu.");
                return null;
            }
            List<Book> found = new List<Book>();

            for (int i = 0; i < books.Count; i++)
            {
                if (from == "bName")
                {
                    if (books[i].GetName().ToLower().Contains(name.ToLower()))
                        found.Add(books[i]);
                }
                else
                {
                    if (books[i].GetAuthor().ToLower().Contains(name.ToLower()))
                        found.Add(books[i]);
                }
            }

            if (found.Count == 0)
                Console.WriteLine("Can't find any book.");
            Console.WriteLine();

            return found;
        }

        public void BorrowBook()
        {
            Console.Clear();
            List<Book> found = SearchBook(display: false);
            if (found == null || found.Count == 0)
            {
                Console.WriteLine("Press any key to return to main menu...");
                Console.ReadKey();
                return;
            }
            Book selected = null;
            char choose = ' ';
            while (true)
            {
                selected = DisplayBooks(unselectable: false, 0, choose, found);
                if (selected.GetCopy() == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\nThis book has 0 copies in the library right now!\nPlease press any key to return...");
                    Console.ReadKey();
                    return;
                }
                else break;

            }
            bool loop = false;
            if (selected == null)
            {
                return;
            }
            do
            {
                Console.Clear();
                Console.WriteLine("You've selected this book:\n{0} by {1}", selected.GetName(), selected.GetAuthor());
                Console.WriteLine("Are you sure to borrow this book?");
                Console.WriteLine("1-)Yes.");
                Console.WriteLine("2-)No.");

                if (loop)
                    Console.WriteLine("Please select a valid option!");
                loop = true;

                if (!libSystem.GetChoose('1', '2', ref choose, null))
                    continue;
            } while (choose == 'p');

            if (choose == '1')
            {
                Random rand = new Random();
                string borrowCode = books.IndexOf(selected).ToString() + selected.GetCopy().ToString() + rand.Next(100, 1000).ToString();
                DateTime now = DateTime.Now;
                DateTime returnTime = now.AddDays(30);
                Borrowed newBorrowed = new Borrowed(selected.GetIsbn(), borrowCode, now, returnTime);
                selected.SetInfo(increase: false);
                borrowedList.Add(newBorrowed);
                WriteBorrow(newBorrowed);
                Console.Clear();
                Console.WriteLine("You've borrowed the book!");
                Console.WriteLine("DON'T FORGET YOUR BORROW CODE: {0}", borrowCode);
                Console.WriteLine("You have to return this book until: {0}", returnTime);
                SaveBooks();
            }
        }
        public void ReturnBook()
        {
            Console.Clear();
            Console.Write("Please enter your Borrow Code:");
            string borrowCode = Console.ReadLine();

            Book selected = null;
            Borrowed borrow = null;

            foreach (Borrowed b in borrowedList)
            {
                if (borrowCode == b.BorrowCode())
                {
                    borrow = b;
                    foreach (Book book in books)
                    {
                        if (b.ISBN() == book.GetIsbn())
                        {
                            selected = book;
                            break;
                        }
                    }
                    if (b != null)
                        break;
                }
            }
            if (borrow == null)
            {
                Console.WriteLine("No Book found!\nPlease press any key to return...");
                Console.ReadKey();
                return;
            }

            char choose = ' ';
            bool loop = false;
            do
            {
                Console.Clear();
                Console.WriteLine("You are returning this book:\n{0} by {1}", selected?.GetName(), selected?.GetAuthor());
                Console.WriteLine("Are you sure to return this book?");
                Console.WriteLine("1-)Yes.");
                Console.WriteLine("2-)No.");

                if (loop)
                    Console.WriteLine("Please select a valid option!");
                loop = true;

                if (!libSystem.GetChoose('1', '2', ref choose, null))
                    continue;
            } while (choose == 'p');

            if (choose == '1')
            {
                borrowedList.Remove(borrow);
                selected?.SetInfo(increase: true);
                SaveBooks();
                SaveBorrows();
            }
        }
        List<Book> PageChange(ref int offSet, char choose, List<Book> found)
        {
            int tempOffSet = offSet;
            offSet += (choose == '8') ? -5 : (choose == '9') ? 5 : 0;
            if (offSet < 0 || offSet > found.Count)
                offSet = tempOffSet;

            int next = Math.Min(found.Count - offSet, 5);

            return found.GetRange(offSet, next);
        }
        public void UnreturnedBooks()
        {
            if (borrowedList.Count == 0)
            {
                Console.WriteLine("No Unreturned books found!\nPress any key to return to back...");
                Console.ReadKey();
                return;
            }
            int offSet = 0;

            List<Book> page = new List<Book>();

            foreach (Borrowed b in borrowedList)
            {
                foreach (Book book in books)
                {
                    if (b.ISBN() == book.GetIsbn())
                    {
                        page.Add(book);
                    }
                }
            }

            char choose = ' ';
            do
            {
                Console.Write("\u001b[2J\u001b[3J");
                Console.Clear();
                int counter = 1;

                for (int i = offSet; i < offSet + 5; i++)
                {
                    Console.WriteLine("{0}-) Name: {1}\nAuthor: {2}\nISBN: {3}\nBorrowed Date: {4}\nReturn Date: {5}\nBorrow Code: {6}\n-----------------------\n",
                        offSet + counter++,
                         page[i].GetName(),
                          page[i].GetAuthor(),
                           page[i].GetIsbn(),
                            borrowedList[i].B_Date(),
                             borrowedList[i].R_Date(),
                             borrowedList[i].BorrowCode());
                    Console.WriteLine("i+1: {0}   PageCount: {1}", i + 1,page.Count);
                    if (i + 1 == page.Count)
                        break;
                }

                if (offSet != 0)
                    Console.WriteLine("8-) Previous Page");
                if (offSet + 5 < page.Count)
                    Console.WriteLine("9-) Next Page");

                Console.WriteLine("x-) Return to main menu.");

                if (!libSystem.GetChoose('8', '9', ref choose, "Display"))
                    continue;



                offSet += (choose == '8') ? -5 : (choose == '9') ? 5 : 0;

                if (choose == 'x')
                    return;

            } while (choose == 'p' || choose == '8' || choose == '9');
        }
        public void GetBooks()
        {
            //Read books and store them in a list named books from a file (I store the file in documents)
            FileStream file = new FileStream(bookPath, FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(file);

            while (!reader.EndOfStream)
            {
                string bName = reader.ReadLine();
                string author = reader.ReadLine();
                string isbn = reader.ReadLine();
                byte copy = Convert.ToByte(reader.ReadLine());
                byte borrow = Convert.ToByte(reader.ReadLine());
                Book newBook = new Book(bName, author, isbn, copy, borrow);
                books.Add(newBook);
            }
            reader.Close();
            file.Close();

            file = new FileStream(borrowPath, FileMode.OpenOrCreate);
            reader = new StreamReader(file);

            while (!reader.EndOfStream)
            {
                string isbn = reader.ReadLine();
                string borrowCode = reader.ReadLine();
                DateTime b_date = Convert.ToDateTime(reader.ReadLine());
                DateTime r_date = Convert.ToDateTime(reader.ReadLine());
                Borrowed borrowed = new Borrowed(isbn, borrowCode, b_date, r_date);
                borrowedList.Add(borrowed);
            }
            reader.Close();
            file.Close();
        }

        public void SaveBooks()
        {
            //Clear the file "path)
            StreamWriter writer = new StreamWriter(bookPath, false);
            writer.Write("");
            writer.Close();
            //Overloaded method "Writebook" takes list of books, not one book.
            WriteBook();
        }

        public void WriteBook()
        {
            //This is overloaded writebook method. It takes list of Book and rewrite them to the existing file.

            FileStream file = new FileStream(bookPath, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);

            for (int i = 0; i < books.Count; i++)
            {
                writer.WriteLine(books[i].GetName());
                writer.WriteLine(books[i].GetAuthor());
                writer.WriteLine(books[i].GetIsbn());
                writer.WriteLine(books[i].GetCopy());
                writer.WriteLine(books[i].GetBorrow());
            }
            writer.Close();
            file.Close();
        }
        public void WriteBook(Book book)
        {
            //This is normal method that took 1 book as parameter. I use this for adding new book.
            //In my researches, I found out I don't need to use FileStream, but I use it just in case if anything happens.

            FileStream file = new FileStream(bookPath, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(book.GetName());
            writer.WriteLine(book.GetAuthor());
            writer.WriteLine(book.GetIsbn());
            writer.WriteLine(book.GetCopy());
            writer.WriteLine(book.GetBorrow());
            writer.Close();
            file.Close();
        }

        public void SaveBorrows()
        {
            StreamWriter writer = new StreamWriter(borrowPath, false);
            writer.Write("");
            writer.Close();
            WriteBorrow();
        }

        public void WriteBorrow()
        {
            //This is overloaded writebook method. It takes list of Book and rewrite them to the existing file.

            FileStream file = new FileStream(borrowPath, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);

            for (int i = 0; i < borrowedList.Count; i++)
            {
                writer.WriteLine(borrowedList[i].ISBN());
                writer.WriteLine(borrowedList[i].BorrowCode());
                writer.WriteLine(borrowedList[i].B_Date());
                writer.WriteLine(borrowedList[i].R_Date());
            }
            writer.Close();
            file.Close();
        }
        public void WriteBorrow(Borrowed borrow)
        {
            FileStream file = new FileStream(borrowPath, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(borrow.ISBN());
            writer.WriteLine(borrow.BorrowCode());
            writer.WriteLine(borrow.B_Date());
            writer.WriteLine(borrow.R_Date());
            writer.Close();
            file.Close();
        }
    }
}