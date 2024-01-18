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
        Library library;
        FileStream file;
        StreamWriter writer;
        string filePath, fileName;



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
                    library.AddBook();
                    break;
                case '2':
                    library.DisplayBooks(unselectable: true, 0, '0', library.ListOfBooks());
                    break;
                case '3':
                    library.SearchBook(display: true);
                    break;
                case '4':
                    library.BorrowBook();
                    break;
                case '5':
                    library.ReturnBook();
                    break;
                case '6':
                    library.UnreturnedBooks();
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

        public void SetCopies(bool increase)
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

        public Book(string bName, string author, string isbn, byte copy, byte borrow)
        {
            this.bName = bName;
            this.author = author;
            this.isbn = isbn;
            this.copy = copy;
            this.borrow = borrow;
        }
    }

    class Library
    {
        List<Book> books = new List<Book>();
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\books.txt";
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
                string bName = SetStrings("Please enter the book's name (Ex. Red Dead): ", "bName");
                if (bName.Length == 0) break;
                string author = SetStrings("Please enter the book's author (Ex. Arthur Morgan): ", "author");
                if (bName.Length == 0) break;
                string isbn = SetStrings("Please enter the book's ISBN (Ex. 9876543210987): ", "isbn");
                if (bName.Length == 0) break;
                byte copy = SetBytes("Please enter how many copies of this book this library have: ");
                if (bName.Length == 0) break;
                byte borrow = SetBytes("Please enter how many copies of this book has been borrowed: ");
                if (bName.Length == 0) break;
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
        byte SetBytes(string message)
        {
            Console.Clear();
            bool error = false;
            while (true)
            {
                if (error)
                    Console.WriteLine("Please enter numeric value. (0 to 255)");
                error = true;

                Console.WriteLine(message);
                string input = Console.ReadLine();
                if (byte.TryParse(input, out byte parse))
                    return Convert.ToByte(input);

            }
        }
        string SetStrings(string message, string from)
        {
            Console.Clear();
            Console.WriteLine("(To cancel adding a new book, press ENTER without write anything.)");
            bool error = false;
            string errorMessage = "";
            while (true)
            {
                if (error)
                    Console.WriteLine(errorMessage);
                errorMessage = "Please enter at least 3 characters.";
                error = true;
                Console.Write(message);
                string str = Console.ReadLine();
                if (from.Equals("isbn"))
                {
                    errorMessage = "Please enter at least 3 numeric values. Only numeric values!";
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
            char foundCount = Convert.ToChar((char)MathF.Min(page.Count + '0', '5'));

            do
            {
                Console.Write("\u001b[2J\u001b[3J");
                Console.Clear();

                for (int i = 0; i < page.Count; i++)
                {
                    Console.WriteLine("{0}-) Name: {1}\nAuthor: {2}\nISBN: {3}\nCopies: {4}\nBorrowed: {5}\n-----------------------\n",
                        (unselectable) ? offSet + i + 1 : i + 1,
                         page[i].GetName(),
                          page[i].GetAuthor(),
                           page[i].GetIsbn(),
                            page[i].GetCopy(),
                             page[i].GetBorrow());
                }

                if (offSet != 0)
                    Console.WriteLine("8-) Previous Page");
                if (offSet + page.Count < found.Count)
                    Console.WriteLine("9-) Next Page");

                Console.WriteLine("x-) Return to main menu.");
                if (!unselectable)
                {
                    Console.Write("Select the one you want to borrow or return: ");
                    if (!libSystem.GetChoose('1', foundCount, ref choose, "Display"))
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

            char choose = ' ';
            Book selected = DisplayBooks(unselectable: false, 0, choose, found);
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
                selected.SetCopies(increase: false);
                SaveBooks();
            }
        }
        List<Book> PageChange(ref int offSet, char choose, List<Book> found)
        {
            int tempOffSet = offSet;
            offSet += (choose == '8') ? -5 : (choose == '9') ? 5 : 0;
            if (offSet < 0 || offSet > found.Count)
                offSet = tempOffSet;

            int next = (int)MathF.Min(found.Count - offSet, 5);

            return found.GetRange(offSet, next);
        }
        public void ReturnBook()
        {
            Console.WriteLine("ReturnBook");
            //TODO: Return a book.
        }

        public void UnreturnedBooks()
        {
            Console.WriteLine("UnreturnedBooks");
            //TODO: Display unreturned books.
        }

        public void WriteBook(Book book)
        {
            //This is normal method that took 1 book as parameter. I use this for adding new book.
            //In my researches, I found out I don't need to use FileStream, but I use it just in case if anything happens.

            FileStream file = new FileStream(path, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(book.GetName());
            writer.WriteLine(book.GetAuthor());
            writer.WriteLine(book.GetIsbn());
            writer.WriteLine(book.GetCopy());
            writer.WriteLine(book.GetBorrow());
            writer.Close();
            file.Close();
        }
        public void WriteBook(List<Book> book)
        {
            //This is overloaded writebook method. It takes list of Book and rewrite them to the existing file.

            FileStream file = new FileStream(path, FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            for (int i = 0; i < book.Count; i++)
            {
                writer.WriteLine(book[i].GetName());
                writer.WriteLine(book[i].GetAuthor());
                writer.WriteLine(book[i].GetIsbn());
                writer.WriteLine(book[i].GetCopy());
                writer.WriteLine(book[i].GetBorrow());
            }
            writer.Close();
            file.Close();
        }
        public void GetBooks()
        {
            //Read books and store them in a list named books from a file (I store the file in documents)
            FileStream file = new FileStream(path, FileMode.OpenOrCreate);
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
        }

        public void SaveBooks()
        {
            //Clear the file "path)
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write("");
            writer.Close();
            //Overloaded method "Writebook" takes list of books, not one book.
            WriteBook(books);
        }

    }
}