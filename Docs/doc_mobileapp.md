# Aplikacja mobilna

Aplikacja mobilna jest przeznaczona dla dwóch typów użytkowników systemu: właścicieli zwierząt oraz opiekunów.
Aplikacja umożliwia właścicielom wystawianie ogłoszeń do opieki nad ich zwierzętami. Daje im także możliwość oceniania opiekunów. Opiekunowie zwierząt mogą w aplikacji wybierać dowole ogłoszenia dostępnych w systemie i zajmować się zwierzętami właścicieli.

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

W widoku rejestracji klient może utworzyć nowe konto w systemie. Wszystkie pola do wypełnienia na tej stronie są obowiązkowe. Dtotyczy to także zdjęcia użytkownika oraz zdjęć dowodu osobistego (zdjęcia te zostaną wykorzystane do weryfikacji użytkownika). Wszystkie pola na w tym widoku podlegają walidacji zgodnej z reprezentowanym typem danych (np. numer telefonu musi składać się z 9 cyfr). Po zarejestrowaniu użytkownik otrzyma informacje, że udało się utworzyć konto i musi on potwierdzić rejestrację (z racji braku implementacji usługi poczty użytkownik musi poczekać jedynie na weryfikację przez administratora systemu).

## Widoki zalogowanego użytkownika
Widoki dla zalogowanego użytkownika składają się z 5 sekcji: Start, Właściciel, Opiekun, Profil oraz Więcej. Użytkownik może dowolnie poruszać się między sekcjami, gdy jest zalogowany w aplikacji.

### Widok główny (Start)
<img src="Screenshot_1738088347.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093060.png" alt="drawing" width="150"/>
<img src="Screenshot_1738093068.png" alt="drawing" width="150"/>

Po zalogowaniu użytkownik jest przenoszony na ekran główny. Na tej stronie wyświetlane jest imię użytkownika oraz jego awatar. Wyświetlane są również jego ogłoszenia, które są w trakcie realizacji (ikona łapy oznacza, że użytkownik jest właścicielem, a ikona teczki oznacza, że jest opiekunem). Aplikacja zapisuje sesje użytkownika w urządzeniu, w związku z czym po wyłączeniu aplikacji i ponownym włączeniu użytkownik zostanie automatycznie przekierowany na ekran Start (jeśli był zalogowany). Strony, które zawierają elementy, które muszą zostać pobrane z serwera wyposażone są w widok wczytywania(kręcące się kółko), który wyświetla się dopóki dane nie zostaną pobrane i przetworzone. W przypadku problemu z pobraniem danych zostanie wyświetlony czerwony kafel z informacją, że nie udało się pobrać danych.

Kliknięcie kafla ogłoszenia wyświetla pełnoekranowy widok ogłoszenia zawierający wszystkie dostępne iformacje o ogłoszeniu. Użytkownik może z tego ekranu wykonywać operacje związane z ogłoszeniem, np. zakończenie lub anulowanie ogłoszenia. Zakończenie lub anulowanie zgłoszenia wyświetla widok umożliwiający dodanie opinii opiekunowi do zwierząt. Dodanie opinii nie jest obowiązkowe.

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

Widok składa się z 4 przycisków kierujących na różne widoki. "Moje adresy" pozwalają zobaczyć, jakie adresy użytkownik ma zapisane. Można tam dodać, edytować i usuwać adresy. Widok "Moje zwierzęta" pozwala dodawać, edytować i usuwać zwierzęta z bazy zwierząt. "Otrzymane opinie" umożwliwiają wyświetlenie opinii, które użytkownik otrzymał, gdy był opiekunem zwierząt. "Wystawione opinie" są opiniami dotyczami innych opiekunów, które wystawił użytkownik.

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

Na początku ogłoszenie jest tworzone w aplikacji przez właściciela. Aby utworzyć ogłoszenie użytkownik musi mieć dodany przynajmniej jeden adres (pierwszy adres jest tworzony podczas rejestracji) oraz przynajmniej jedno zwierzę. Gdy te warunki są spełnione użytkownik może utworzyć ogłoszenie. Podczas tworzenia ogłoszenia właściciel podaje kwotę, jaką zapłaci opiekunowi. Może zaznaczyć opcję, że cena jest do negocjacji, co pozwoli na indywidualne dopasowanie ceny dla potencjalnego opiekuna. Podczas tworzenia ogłoszenia użytkownik wybiera zwierzę oraz adres, skąd należy odebrać zwierzę. Wymagany jest również opis ogłoszenia oraz czas, w którym ogłoszenie powinno być realizowane. Po stworzeniu ogłoszenia właściciel ma możliwość edytowania lub usunięcia ogłoszenia do momentu, kiedy zgłosi się opiekun. Gdy opiekun zgłosi chęć zajęcia się zwierzęciem właściciel może to zgłoszenie odrzucić lub przyjąć. Opiekun również może anulować zgłoszenie do momentu akceptacji przez właściciela. Po akceptacji zgłoszenia właściciel rozpoczyna zgłoszenie, kiedy opiekun odbierze od niego zwierzę. Po powrocie zwierzęcia do właściciela, wtedy kończy on zgłoszenie. Wybranie opcji anulowanie oznacza, że coś poszło nie tak podczas realizacji zgłoszenia. Po zakończeniu lub anuowaniu zgłoszenia właściciel ma możliwość wystawienia opinii opiekunowi. Podczas wystawiania opinii musi podać ocenę oraz może uzupełnić opis opinii. Opinie do anulowanych zgłoszeń są kluczowe w procesie banowania użytkowników. Jeżeli zgłoszenie zostało anulowane i opiekun dostał niską ocenę, może to świadczyć o niewłaściwym zachowaniu użytkownika, i być może należy go zbanować.
