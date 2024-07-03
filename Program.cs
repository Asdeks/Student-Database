
using System.Xml.Serialization;

namespace ConsoleApp2
{
    public class Student
    {
        public string KrestniJmeno { get; set; }          // Jméno studenta
        public string Prijmeni { get; set; }              // Příjmení studenta
        public string Obor { get; set; }                  // Obor studia
        public int Id { get; set; }                       // Identifikační číslo studenta
        public int RokNarozeni { get; set; }              // Rok narození studenta

        // Kontrola správnosti údajů ve struktuře
        public bool JePlatny()
        {
            return !string.IsNullOrWhiteSpace(KrestniJmeno) &&
                   !string.IsNullOrWhiteSpace(Prijmeni) &&
                   Id > 0 &&
                   RokNarozeni > 0;
        }
    }

    // Třída pro obalení seznamu studentů.
    public class ObalStudentu
    {
        public List<Student> Studenti { get; set; } = new List<Student>();   // Seznam studentů
    }

    class Program
    {
        // Konstanta určující název souboru XML pro ukládání dat
        private const string CestaKsouboru = "Studenti.xml";

        

        // Vytvoření nového studenta s ověřením vstupních údajů
        public static Student VytvorStudenta(List<Student> studenti)
        {
            Student student = new Student();

            Console.WriteLine("Zadejte křestní jméno:");
            student.KrestniJmeno = Console.ReadLine().Trim();

            while (string.IsNullOrWhiteSpace(student.KrestniJmeno) || student.KrestniJmeno.Any(char.IsDigit))
            {
                Console.WriteLine("Chyba: Křestní jméno nemůže být prázdné nebo obsahovat číslice. Zadejte znovu:");
                student.KrestniJmeno = Console.ReadLine().Trim();
            }

            Console.WriteLine("Zadejte příjmení:");
            student.Prijmeni = Console.ReadLine().Trim();

            while (string.IsNullOrWhiteSpace(student.Prijmeni) || student.Prijmeni.Any(char.IsDigit))
            {
                Console.WriteLine("Chyba: Příjmení nemůže být prázdné nebo obsahovat číslice. Zadejte znovu:");
                student.Prijmeni = Console.ReadLine().Trim();
            }

            Console.WriteLine("Zadejte obor:");
            student.Obor = Console.ReadLine().Trim();

            if (studenti.Count == 0)
            {
                student.Id = 1;
            }
            else
            {
                student.Id = studenti.Max(s => s.Id) + 1;
            }

            Console.WriteLine("Zadejte rok narození:");
            while (true)
            {
                if (int.TryParse(Console.ReadLine().Trim(), out int rokNarozeni) && rokNarozeni > 0)
                {
                    student.RokNarozeni = rokNarozeni;
                    break;
                }
                Console.WriteLine("Chyba: Neplatný vstup. Zadejte platný rok narození:");
            }

            return student;
        }

        // Tisk detailů studenta po ID
        public static void TiskniStudentaID(Student student)
        {

            if (student != null && student.JePlatny())
            {

                Console.WriteLine($"{student.KrestniJmeno,-15} {student.Prijmeni,-15} {student.Obor,-15} {student.RokNarozeni,-15} {student.Id,-15}");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neplatné údaje studenta.");
            }
        }

        // Odstranění studenta z seznamu
        public static void OdstranStudenta(List<Student> studenti)
        {
            Console.WriteLine("Zadejte ID studenta k odstranění:");
            int id;
            while (!int.TryParse(Console.ReadLine().Trim(), out id) || id <= 0)
            {
                Console.WriteLine("Chyba: Neplatný vstup. Zadejte platné ID:");
            }

            var studentKodstraneni = studenti.FirstOrDefault(s => s.Id == id);
            if (studentKodstraneni != null)
            {
                studenti.Remove(studentKodstraneni);

                // Resorting IDs
                for (int i = 0; i < studenti.Count; i++)
                {
                    studenti[i].Id = i + 1;
                }

                UlozDoXml(studenti);
            }
            else
            {
                Console.WriteLine("Student s ID {0} nebyl nalezen.", id);
            }
        }

        // Uložení seznamu studentů do XML souboru
        public static void UlozDoXml(List<Student> studenti)
        {
            ObalStudentu obal = new ObalStudentu { Studenti = studenti };

            XmlSerializer serializer = new XmlSerializer(typeof(ObalStudentu));
            using (TextWriter writer = new StreamWriter(CestaKsouboru))
            {
                serializer.Serialize(writer, obal);
            }
        }

        // Načtení seznamu studentů z XML souboru
        public static List<Student> NactiZXml()
        {
            if (File.Exists(CestaKsouboru))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObalStudentu));
                using (TextReader reader = new StreamReader(CestaKsouboru))
                {
                    ObalStudentu obal = (ObalStudentu)serializer.Deserialize(reader);
                    return obal.Studenti;
                }
            }
            else
            {
                Console.WriteLine("Soubor neexistuje.");
                return new List<Student>();
            }
        }
        // Hledaní studenta po Přijmeni.
        public static List<Student> NajdiStudentaPodlePrijmeni(List<Student> studenti, string prijmeni)
        {
            return studenti.Where(s => s.Prijmeni.Equals(prijmeni, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        public static void Main(string[] args)
        {
            List<Student> studenti = new List<Student>();
            char volba;

            // Hlavní menu
            do
            {
                Console.Clear();

                Console.WriteLine("---------------MENU---------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Přidat studenta [a]");
                Console.WriteLine("Vyhledat studenta po ID [b]");
                Console.WriteLine("Otevřit seznam [c]");
                Console.WriteLine("Vyhledat studenta po Prijmeni [g]");
                Console.WriteLine("Odstranit studenta [d]");
                Console.WriteLine("Uložit do souboru [i]");
                Console.WriteLine("Načíst ze souboru [f]");
                Console.WriteLine("Konec [e]");
                Console.Write("Zadejte volbu: ");

                volba = char.ToLower(Console.ReadKey().KeyChar);

                switch (volba)
                {
                    // řádek, který volá funkci "VytvorStudenta".
                    case 'a':
                        Console.Clear();
                        Console.WriteLine("Přidat studenta");
                        Console.WriteLine("------------------------");
                        studenti.Add(VytvorStudenta(studenti));
                        break;

                    // řádek, který volá funkci "TiskniStudentaID".

                    case 'b':
                        Console.Clear();

                        int id;
                        if (studenti.Count == 0)
                        {
                            Console.WriteLine("V seznamu nejsou žádní studenti. Pro vyhledání studenta nejdříve přidejte nového studenta.");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Vyhledat studenta");
                            Console.WriteLine("------------------------");
                            Console.WriteLine("Zadejte ID k vyhledání:");


                            while (!int.TryParse(Console.ReadLine().Trim(), out id) || id <= 0)
                            {

                                Console.WriteLine("Chyba: Neplatný vstup. Zadejte platné ID:");
                                break;
                            }

                            var nalezenyStudent = studenti.FirstOrDefault(s => s.Id == id);
                            Console.WriteLine($"{"Jméno",-15} {"Příjmení",-15} {"Obor",-15} {"Rok",-15} {"ID",-15}");
                            TiskniStudentaID(nalezenyStudent);

                            Console.ReadKey();
                            break;
                        }

                    // řádek, který volá funkci "TiskniStudentaID".

                    case 'c':
                        Console.Clear();
                        Console.WriteLine("SEZNAM STUDENTU");
                        Console.WriteLine("------------------------");
                        Console.WriteLine($"{"Jméno",-15} {"Příjmení",-15} {"Obor",-15} {"Rok",-15} {"ID",-15}");
                        foreach (var student in studenti)
                        {
                            TiskniStudentaID(student);
                        }
                        Console.ReadKey();
                        break;

                    // řádek, který volá funkci "NajdiStudentaPodlePrijmeni".

                    case 'g':
                        Console.Clear();
                        Console.WriteLine("Najít studenta podle příjmení");
                        Console.WriteLine("------------------------");
                        Console.WriteLine("Zadejte příjmení:");
                        string prijmeniHledani = Console.ReadLine().Trim();

                        var nalezeniStudenti = NajdiStudentaPodlePrijmeni(studenti, prijmeniHledani);

                        if (nalezeniStudenti.Any())
                        {
                            Console.WriteLine($"{"Jméno",-15} {"Příjmení",-15} {"Obor",-15} {"Rok",-15} {"ID",-15}");
                            foreach (var student in nalezeniStudenti)
                            {
                                TiskniStudentaID(student);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Student s tímto příjmením nebyl nalezen.");
                        }
                        Console.ReadKey();
                        break;

                    // řádek, který volá funkci "Smazat studenta".

                    case 'd':
                        Console.Clear();
                        Console.WriteLine("OdstranStudenta");
                        Console.WriteLine("------------------------");
                        OdstranStudenta(studenti);
                        break;

                    // řádek, který volá funkci "UlozDoXml".

                    case 'i':
                        Console.Clear();
                        Console.WriteLine("Uložit do souboru");
                        Console.WriteLine("------------------------");
                        UlozDoXml(studenti);
                        Console.WriteLine("Uloženo");
                        Console.Read();
                        break;

                    // řádek, který volá funkci "NactiZXml"

                    case 'f':
                        Console.Clear();
                        Console.WriteLine("Načíst ze souboru");
                        Console.WriteLine("------------------------");
                        studenti = NactiZXml();
                        Console.WriteLine("Načtěno");
                        Console.Read();
                        break;

                    // řádek, který zavíra program

                    case 'e':
                        Console.Clear();
                        Console.WriteLine("Ukončuji...");
                        break;

                    // Pokud je tento řádek zadán nesprávně, odešle zprávu "Neplatná volba!".

                    default:
                        Console.WriteLine("\nNeplatná volba!");
                        Console.ReadKey();
                        break;
                }

            } while (volba != 'e');
        }
    }
}