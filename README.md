# Analizator Danych Rynkowych

## Opis
**Analizator Danych Rynkowych** to aplikacja konsolowa napisana w języku C#. Program umożliwia automatyczne pobieranie danych z wybranych stron internetowych oraz zapisywanie wyników w plikach tekstowych. Aplikacja obsługuje zadania cykliczne, które można definiować.

## Funkcje
- **Dodawanie zadań**: Możliwość definiowania zadań, które określają, jakie dane mają być pobierane z podanej strony internetowej.
- **Migawka strony**: Tworzenie migawki (snapshot) całej strony HTML i zapisywanie jej do pliku.
- **Harmonogram zadań**: Automatyczne wykonywanie zadań w określonych odstępach czasu.

## Użycie
1. Po uruchomieniu aplikacji kliknij `M` w celu wyświetlenia menu z następującymi opcjami:
   - **1**: Dodaj nowe zadanie do harmonogramu. Aplikacja poprosi o podanie szczegółów, takich jak nazwa zadania, częstotliwość, URL, XPath oraz inne parametry.
   - **2**: Usuń istniejące zadanie. Wprowadź nazwę zadania, które chcesz usunąć.
   - **3**: Wykonaj migawkę strony. Podaj URL strony, aby zapisać jej zawartość HTML do pliku `snapshot.html`.
   - **4**: Wyświetl czasy do wykonania zadań. Zobacz, ile czasu pozostało do kolejnego wykonania każdego zadania.
   - **Enter**: Zakończ działanie programu.
2. Wybierz odpowiednią opcję, naciskając odpowiedni klawisz na klawiaturze.
3. Postępuj zgodnie z instrukcjami wyświetlanymi w konsoli, aby wprowadzić wymagane dane lub potwierdzić operacje.
4. Wszystkie operacje są logowane w pliku `log.txt` w celu śledzenia działań użytkownika.

## Struktura Plików
- **tasks.txt**: Plik przechowujący zdefiniowane zadania.
- **log.txt**: Plik logów, w którym zapisywane są wszystkie operacje.
- **snapshot.html**: Plik zawierający migawkę strony HTML.

## Przykład Zadania
Przykładowe zadanie w pliku `tasks.txt`:
- `task1`: Nazwa zadania.
- `2`: Częstotliwość wykonywania zadania w minutach.
- `https://steamcommunity.com/market/search?appid=730`: URL strony.
- `/html/body/div[1]/div[7]/div[2]/div[1]/div[4]/div[2]/div[2]/div/div[1]`: XPath do elementu HTML.
- `<div class="market_listing_row market_recent_listing_row market_listing_searchresult"`: Znacznik nazwy do odczytu.
- `48`: Przesunięcie kursora odczytu nazwy.
- `">`: Znak kończący.
- `<span class="market_listing_num_listings_qty" data-qty="`: Znacznik wartości do odczytu.
- `0`: Przesunięcie kursora odczytu wartości.
- `"`: Znak kończący.

