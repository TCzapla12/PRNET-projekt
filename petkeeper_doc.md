#### Sławomir Batruch, Tomasz Czaplicki

# System do opieki nad zwierzętami - PetKeeper

# Wstęp 

Stworzony projekt jest systemem do zlecania oraz udzielania opieki nad zwierzętami. Ideą jest, że użytkownik potrzebujący opieki nad zwierzęciem wystawia ogłoszenie wraz z informacjami potrzebnymi do jej udzielenia (m.in. okres, adres odbioru, proponowana cena). Następnie inni zarejestrowani użytkownicy w aplikacji mogą przyjąć ogłoszenie, umawiając się dokładniej z wystawiającym na podjęcie opieki. Nad aplikacją czuwa administrator systemu, który odpowiada za sprawdzanie nowych kont oraz ma możliwość ich blokowania na podstawie negatywnych opinii klientów. 

# Architektura

System został stworzony z wykorzystaniem dostępnych frameworków i modułów w C#/.NET. Architektura aplikacji składa się z 5 elementów: dwóch aplikacji, back-endu wraz
z repozytorium plików oraz bazy danych. Poszczególne elementy systemu zostały opisane w dalszej części dokumentacji.

<img src="Screenshot_1.png" alt="drawing" width="600"/>

Wszystkie elementy architektury poza aplikacją mobilną zostały skonteneryzowane i mogą być uruchamiane za pomocą narzędzia do konteneryzacji, np. Docker Desktop.

<img src="Screenshot_2.png" alt="drawing" width="600"/>

# Aplikacja mobilna

Aplikacja mobilna jest przeznaczona dla dwóch typów użytkowników systemu: właścicieli zwierząt oraz opiekunów.
Aplikacja umożliwia właścicielom wystawianie ogłoszeń do opieki nad ich zwierzętami. Daje im także możliwość oceniania opiekunów. Opiekunowie zwierząt mogą w aplikacji wybierać dowolne ogłoszenia dostępnych w systemie i zajmować się zwierzętami właścicieli.

Widoki aplikacji możemy podzielić na dwie kategorie - widoki dla użytkowników niezalogowanych i zalogowanych.

## Widoki dla użytkownika niezalogowanego
Dla niezalogowanego użytkownika dostępne są 3 widoki: Logowanie, Rejestracja, Resetowanie Hasła.

<img src="Screenshot_1738087487.png" alt="drawing" width="150"/>
<img src="Screenshot_1738087491.png" alt="drawing" width="150"/>
<img src="Screenshot_1738087498.png" alt="drawing" width="150"/>
<img src="Screenshot_1738087501.png" alt="drawing" width="150"/>

### Widok Logowania

Na widoku logowania użytkownik może przejść do widoków dostępnych jedynie dla zalogowanych użytkowników po podaniu poprawnego adresu e-mail oraz hasła. Pola do wpisania znajdujące się na tym ekranie są wyposażone w proces walidacji adresu email oraz hasła przed wysłaniem żądania do serwera aplikacji. Użytkownik, który nie posiada zweryfikowanego konta albo został zbanowany nie uzyska dostępu do widoków dla zalogowanych użytkowników. Zamiast tego otrzyma komunikat, dlaczego nie może się zalogować. Na tym ekranie użytkownik może również przejść do rejestracji konta oraz do resetowania hasła.

### Widok Resetowania Hasła

Widok resetowania hasła nie jest w pełni funkcjonalny, tzn. w ramach projektu nie implementowaliśmy mechanizmów poczty, w związku z czym nie ma możliwości wysłania maila z prośbą o reset hasła. Po naciśnięciu przycisku "Wyślij" dostaniemy informację, że metoda nie została zaimplementowana.

### Widok Rejestracji

W widoku rejestracji klient może utworzyć nowe konto w systemie. Wszystkie pola do wypełnienia na tej stronie są obowiązkowe. Dotyczy to także zdjęcia użytkownika oraz zdjęć dowodu osobistego (zdjęcia te zostaną wykorzystane do weryfikacji użytkownika). Wszystkie pola na w tym widoku podlegają walidacji zgodnej z reprezentowanym typem danych (np. numer telefonu musi składać się z 9 cyfr). Po zarejestrowaniu użytkownik otrzyma informacje, że udało się utworzyć konto i musi on potwierdzić rejestrację (z racji braku implementacji usługi poczty użytkownik musi poczekać jedynie na weryfikację przez administratora systemu).

## Widoki zalogowanego użytkownika
Widoki dla zalogowanego użytkownika składają się z 5 sekcji: Start, Właściciel, Opiekun, Profil oraz Więcej. Użytkownik może dowolnie poruszać się między sekcjami, gdy jest zalogowany w aplikacji.

### Widok główny (Start)
<img src="Screenshot_1738088347.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093060.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093068.png" alt="drawing" width="150"/>

Po zalogowaniu użytkownik jest przenoszony na ekran główny. Na tej stronie wyświetlane jest imię użytkownika oraz jego awatar. Wyświetlane są również jego ogłoszenia, które są w trakcie realizacji (ikona łapy oznacza, że użytkownik jest właścicielem, a ikona teczki oznacza, że jest opiekunem). Aplikacja zapisuje sesje użytkownika w urządzeniu, w związku z czym po wyłączeniu aplikacji i ponownym włączeniu użytkownik zostanie automatycznie przekierowany na ekran Start (jeśli był zalogowany). Strony, które zawierają elementy, które muszą zostać pobrane z serwera wyposażone są w widok wczytywania(kręcące się kółko), który wyświetla się dopóki dane nie zostaną pobrane i przetworzone. W przypadku problemu z pobraniem danych zostanie wyświetlony czerwony kafel z informacją, że nie udało się pobrać danych.

Kliknięcie kafla ogłoszenia wyświetla pełnoekranowy widok ogłoszenia zawierający wszystkie dostępne informacje o ogłoszeniu. Użytkownik może z tego ekranu wykonywać operacje związane z ogłoszeniem, np. zakończenie lub anulowanie ogłoszenia. Zakończenie lub anulowanie zgłoszenia wyświetla widok umożliwiający dodanie opinii opiekunowi do zwierząt. Dodanie opinii nie jest obowiązkowe.

### Widok Właściciel

<img src="Screenshot_1738088403.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093132.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093144.png" alt="drawing" width="150"/>

Widok właściciela umożliwia dodawanie ogłoszeń i zarządzanie nimi z punktu widzenia właściciela zwierząt. Można tutaj dodawać, edytować i usuwać ogłoszenia, a także zobaczyć ogłoszenia zakończone i anulowane. 

Sekcja "Moje ogłoszenia" wyświetla wszystkie ogłoszenia, które nie zostały zakończone lub anulowane. Po kliknięciu na kafel z takim ogłoszeniem wyświetla się więcej informacji na jego temat (podobnie, jak na widoku głównym).

### Widok Opiekun

<img src="Screenshot_1738088471.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093144.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093252.png" alt="drawing" width="150"/>

Widok opiekuna umożliwia przyjmowanie ogłoszeń przez użytkownika. Na tym widoku można przeglądać aktywne ogłoszenia, szukać nowych ogłoszeń, a także zobaczyć ogłoszenia zakończone i anulowane. 

Ekran wyszukiwania ogłoszeń wyposażony jest w filtrowanie wyników względem profitu oraz daty realizacji ogłoszenia. Można także wpisać miasto do jakiego ma być ograniczone wyszukiwanie. Naciśnięcie ikony lokalizacji powoduje wpisanie w polu "Wyszukaj miejscowość..." aktualną miejscowość z systemu GPS.

### Widok Profil

<img src="Screenshot_1738088535.png" alt="drawing" width="150"/>
<img src="Screenshot_1738092661.png" alt="drawing" width="150"/>
<img src="Screenshot_1738092911.png" alt="drawing" width="150"/>
<img src="Screenshot_1738092805.png" alt="drawing" width="150"/>

Widok składa się z 4 przycisków kierujących na różne widoki. "Moje adresy" pozwalają zobaczyć, jakie adresy użytkownik ma zapisane. Można tam dodać, edytować i usuwać adresy. Widok "Moje zwierzęta" pozwala dodawać, edytować i usuwać zwierzęta z bazy zwierząt. "Otrzymane opinie" umożliwiają wyświetlenie opinii, które użytkownik otrzymał, gdy był opiekunem zwierząt. "Wystawione opinie" są opiniami dotyczącymi innych opiekunów, które wystawił użytkownik.

### Widok Więcej

<img src="Screenshot_1738088547.png" alt="drawing" width="150"/>

Ostatni widok składa się jedynie z przycisku umożliwiającego wylogowanie z aplikacji. Po wylogowaniu użytkownikowi pokaże się strona z ekranem logowania. Docelowo ekran więcej może zostać wyposażony w więcej przycisków, np. przycisku do zmiany ustawień aplikacji lub rzadziej używanych przycisków.

## Dodatkowe elementy dla widoków
<img src="Screenshot_1738091858.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093429.png" alt="drawing" width="150"/>
<img src="Screenshot_1738091976.png" alt="drawing" width="150"/>
<img src="Screenshot_1738091932.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093412.png" alt="drawing" width="150"/>


Do aplikacji stworzono wiele własnych elementów generycznych, które zostały wykorzystane w wielu miejscach w aplikacji. Przykładem takich elementów są kafle ogłoszeń, kafle błędów, pola do wpisywania z walidacją, czy też widoki potwierdzające poprawne lub niepoprawne wykonanie zapytań gRPC.

## Cykl życia ogłoszenia

Na początku ogłoszenie jest tworzone w aplikacji przez właściciela. Aby utworzyć ogłoszenie użytkownik musi mieć dodany przynajmniej jeden adres (pierwszy adres jest tworzony podczas rejestracji) oraz przynajmniej jedno zwierzę. Gdy te warunki są spełnione użytkownik może utworzyć ogłoszenie. Podczas tworzenia ogłoszenia właściciel podaje kwotę, jaką zapłaci opiekunowi. Może zaznaczyć opcję, że cena jest do negocjacji, co pozwoli na indywidualne dopasowanie ceny dla potencjalnego opiekuna. Podczas tworzenia ogłoszenia użytkownik wybiera zwierzę oraz adres, skąd należy odebrać zwierzę. Wymagany jest również opis ogłoszenia oraz czas, w którym ogłoszenie powinno być realizowane. Po stworzeniu ogłoszenia właściciel ma możliwość edytowania lub usunięcia ogłoszenia do momentu, kiedy zgłosi się opiekun. Gdy opiekun zgłosi chęć zajęcia się zwierzęciem właściciel może to zgłoszenie odrzucić lub przyjąć. Opiekun również może anulować zgłoszenie do momentu akceptacji przez właściciela. Po akceptacji zgłoszenia właściciel rozpoczyna zgłoszenie, kiedy opiekun odbierze od niego zwierzę. Po powrocie zwierzęcia do właściciela, wtedy kończy on zgłoszenie. Wybranie opcji anulowanie oznacza, że coś poszło nie tak podczas realizacji zgłoszenia. Po zakończeniu lub anulowaniu zgłoszenia właściciel ma możliwość wystawienia opinii opiekunowi. Podczas wystawiania opinii musi podać ocenę oraz może uzupełnić opis opinii. Opinie do anulowanych zgłoszeń są kluczowe w procesie banowania użytkowników. Jeżeli zgłoszenie zostało anulowane i opiekun dostał niską ocenę, może to świadczyć o niewłaściwym zachowaniu użytkownika. Administrator wtedy może podjąć decyzję o zablokowaniu takiego użytkownika.

# API oraz baza danych
Baza danych służy do przechowywania danych użytkowników, ich ogłoszeń, zwierząt, adresów i opinii. Aplikacja mobilna oraz administratora kontaktuje się z API, które odpytuje bazę danych wykonując odpowiednie kwerendy. Zaimplementowane są odpowiednie mechanizmy zapewniające poprawność danych według ustalonych zasad i schematów.

## Schemat bazy danych
Poniżej przedstawione są relacje tabel. Każda tabela zawiera klucz główny będący w formacie UUID. W większości wypadków użyta jest relacja typu jeden do wielu, nawiązując do ID danego obiektu. Wyjątkami są opinie nawiązujące do ogłoszenia, oraz ogłoszenie do adresu i użytkownika relacją typu jeden do jednego. Daty reprezentowane są za pomocą czasu UNIX ze względu na brak potrzeby zaawansowanych operacji na datach jak np. przetwarzanie wielu stref czasowych

![Schemat bazy danych](db_schema.png)

## Struktura API
API zawiera operacje CRUD dla każdej z tabel. Zawartość wiadomości zwrotnej jest zależna od typu punktu końcowego oraz typu zasobu na którego temat informacja jest zwracana:
- Punkty końcowe odczytujące zasoby zwracają pełną informację o zapytanym zasobie, jednak bez podawania pełnych struktur danych pól nawiązujących do innych tabel. Aby uzyskać obiekt do którego inny obiekt nawiązuje, należy wykonać zapytanie do innego punktu końcowego
- Punkty końcowe tworzące zasoby zwracają jedynie identyfikator stworzonego zasobu. W większości wypadków jest to ID
- Punkty końcowe usuwające zasoby mają taką samą logikę co do zwracanych wartości jak punkty końcowe tworzące zasoby. Zwrócenie ID potwierdza poprawne usunięcie, natomiast błąd oznacza niepowodzenie
- Punkty końcowe aktualizujące zasoby zwracają jedynie pola, które rzeczywiście zostały zmienione w bazie danych. W wypadku jeżeli w żądaniu podano pole, którego nie można zmienić lub jego wartość jest taka sama w bazie danych, w odpowiedzi zwrotnej pole to nie będzie obecne
Istnieją pojedyncze punkty końcowe poza klasycznym CRUD, np. punkt końcowy zwracający informację o każdym użytkowniku, wraz z danymi wrażliwymi (zdjęcie dokumentu tożsamości, PESEL). Użytkownik bez uprawnień jest w stanie uzyskać informacje o użytkowniku bez danych wrażliwych.
### Parametry w nagłówkach
Ważną cechą jest możliwość wywnioskowania ID użytkownika przy zapytaniach na podstawie metadanych tokenu JWT. Nie jest potrzebne podawanie ID użytkownika w parametrach żądania. W wypadku braku podania parametru ID użytkownika, parametr ten jest ekstraktowany z tokenu. Mechanizm ten jest połączony z brakiem pozwolenia na uzyskiwanie danych innych użytkowników przez nie-administratorów w wypadków punktów końcowych gdzie możliwe jest oddzielne podanie parametru ID użytkownika. W takim wypadku nawet jeżeli parametr ID użytkownika podany jest w żądaniu, zostanie on nadpisany przez ID z tokenu.
### Autoryzacja
Autoryzacja zaimplementowana jest za pomocą tokenów JWT zawierających metadane. Najważniejszymi metadanymi jest wspomniany ID użytkownika, oraz jego rola (czy jest administratorem czy nie). Autoryzacja odbywa się poprzez porównanie pary e-mail + hash hasła do informacji obecnej w bazie danych. Dodatkowo sprawdzane jest czy użytkownik jest zweryfikowany i czy nie jest zablokowany. W wypadku blokady czy braku weryfikacji, token nie jest wydawany i użytkownik nie może korzystać z serwisu ze względu na obowiązkową autoryzację przy wykonywaniu żądań do API.

# Widok administratora w aplikacji web
Trzecią częścią projektu jest aplikacja webowa napisana w frameworku Razor. Służy do zarządzania serwisem poprzez sprawdzanie użytkowników oraz ich ogłoszeń i opinii. Na podstawie obrazu dokumentu użytkownika administrator podejmuje decyzję o weryfikacji użytkownika, natomiast na podstawie nieodpowiednich treści ogłoszeń i opinii administrator może zablokować użytkownika. Dostępne są następujące widoki:
- Strona logowania. Wymagane jest podanie loginu i hasła użytkownika, który posiada status administratora. W przeciwnym wypadku wystąpi błąd
<div style="text-align: center;">
  <img src="logowanie.png" alt="logowanie" />
</div>

- Strona główna. Zawiera nawigację do innych stron: widoku wszystkich użytkowników, wyszukiwania użytkowników, widoku wszystkich opinii oraz wyszukiwania ogłoszeń
<div style="text-align: center;">
  <img src="strona_glowna.png" alt="strona glowna" />
</div>

- Strona "Użytkownicy" pokazuje listę wszystkich użytkowników z podstawowymi informacjami o ich statusie oraz opcją kliknięcia w adres e-mail aby przekierować na widok pokazujący pełne informacje o pojedynczym użytkowniku
<div style="text-align: center;">
  <img src="lista_uzytkownikow.png" alt="lista uzytkownikow" />
</div>

- Strona "Wyszukaj Użytkownika" pozwala na wyszukanie użytkownika na podstawie ID, adresu email lub nazwy użytkownika oraz pokazanie pełnej informacji o użytkowniku, wraz z danymi potrzebnymi do weryfikacji (PESEL, zdjęcie dokumentu tożsamości)
<div style="text-align: center;">
  <img src="uzytkownik.png" alt="uzytkownik" />
</div>

- Strona "Opinie" pozwalająca zobaczyć wszystkie opinie, wraz z hiperłączami do powiązanych użytkowników (opiekun i autor) oraz powiązanego ogłoszenia
<div style="text-align: center;">
  <img src="opinie.png" alt="opinie" />
</div>

- Strona "Ogłoszenia" pozwalająca zobaczyć wszystkie ogłoszenia, również z hiperłączami do powiązanych użytkowników
<div style="text-align: center;">
  <img src="ogloszenia.png" alt="ogloszenia" />
</div>