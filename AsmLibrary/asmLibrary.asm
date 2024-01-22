; Wykrywanie Krawedzi operatorem Sobela
; Dawid Kacza

; Wersja 0.2 (19.11.2023)
;	Przekazywanie parametrow z programu wysokiego poziomu
; Wersja 0.3 (26.11.2023)
;	Podstawowa iteracja po bitmapie
; Wersja 0.5 (10.12.2023)
;	Implementacja wlasciwego wykrywania krawedzi
; Wersja 0.6 (16.12.2023)
;	Implementacja instrukcji SSE w procedurze GET_GRAYSCALE
; Wersja 0.7 (19.01.2024)
;	Zmiana struktury programu
;	Przeniesienie konwersji na czarno-bialy do jezyka wysokiego poziomu
;	Implementacja instrukcji SSE w glownej czesci programu

; Stale
.DATA
; Kazda z macierzy wykrywania krawedzi przechowywana bedzie w 2 rejestrach XMM
; Macierz przeksztalcenia horyzontalna
; -1  0  1
; -2  0  2
; -1  0  1  
HORIZONTAL_MATRIX_1		DD -1, -0, 1, -2		; XMM0
HORIZONTAL_MATRIX_2		DD 2, -1, 0, 1			; XMM1

; Macierz przeksztalcenia wertykalna
;  1  2  1
;  0  0  0
; -1 -2 -1
VERTICAL_MATRIX_1		DD 1, 2, 1, 0			; XMM2
VERTICAL_MATRIX_2		DD 0, -1, -2, -1		; XMM3


.CODE
; Procedura CHECK_ROW_COND
; Opis dzialania:
;	Procedura sprawdza warunek (aktualnyY <= koncowyY)
; Parametry wejsciowe:
;	R12 -> Aktualnegy rzad
;	R11 -> Koncowy rzad
; Parametry wyjsciowe:
;	RAX -> 1 jezeli warunek spelniony, 0 jezeli nie spelniony
; Uzywane rejestry 
;	R8
; Niszczone rejestry
;	BRAK, zawartosc rejestu R8 zapisywana jest na stosie

CHECK_ROW_COND PROC
							PUSH R8							; Zapisz na stosie zawartosc rejestru R8, bedzie sluzyl na obliczenia
							MOV RAX, 0						; Na poczatku zakladamy falsz
							MOV R8, R11						; Przenies rzad koncowy do R8
							SUB R8, R12						; Odejmij od rzadu koncowego rzad aktualny
							JS FINISH_CHECK_ROW_COND					
							MOV RAX, 1						; Jezeli koncowyY - aktualnyY >= 0, zwroc prawde
FINISH_CHECK_ROW_COND:		POP R8							; Zdejmij ze stosu zmienne
							RET

CHECK_ROW_COND ENDP


; Procedura CHECK_EDGE_ROW_COND
; Opis dzialania:
;	Procedura sprawdza warunek (aktualnyY == 0 || aktualnyY == wysokosc - 1)
; Parametry wejsciowe:
;	R12 -> Aktualny rzad
;	R9 -> Wysokosc bitmapy
; Parametry wyjsciowe:
;	RAX -> 1 jezeli warunek spelniony, 0 jezeli nie spelniony
; Uzywane rejestry 
;	R8
; Niszczone rejestry
;	BRAK, zawartosc rejestu R8 zapisywana jest na stosie

CHECK_EDGE_ROW_COND PROC
							PUSH R8							; Zapisz na stosie zawartosc rejestru R8, bedzie sluzyl na obliczenia
							MOV RAX, 1						; Na poczatku zakladamy prawde

							MOV R8, R12						; Przenies aktualny rzad do R8
							CMP R8, 0						; Porownaj aktualny rzad z 0
							JE FINISH_CHECK_EDGE_ROW_COND	; Jezeli aktualnyY == 0, konczymy

							MOV R8, R9						; Przenies wysokosc do R8
							SUB R8, 1						; Odejmij 1 od rzadu koncowego w R8
							SUB R8, R12						; Odejmij od rzadu koncowego w R8 aktualny rzad
							JZ FINISH_CHECK_EDGE_ROW_COND

							MOV RAX, 0						; Dla spelnionego warunku wpisz 1 do RAX

FINISH_CHECK_EDGE_ROW_COND:	POP R8							; Zdejmij ze stosu zmienne
							RET

CHECK_EDGE_ROW_COND ENDP


; Procedura CHECK_COL_COND
; Opis dzialania:
;	Procedura sprawdza warunek (aktualnyX < szerokosc)
; Parametry wejsciowe:
;	R10 -> Aktualna kolumna
;   R8 -> Szerokosc obrazu
; Parametry wyjciowy:
;	RAX -> 1, jezelu warunek spelniony, 0 jezeli nie spelniony
; Niszczone rejestry:
;	BRAK, zawartosc rejestru R9 na ktorym przeprowadzane sa obliczenia odkladana jest na stos
CHECK_COL_COND PROC
							PUSH R9							; Zapisz na stosie zawartosc R9
							MOV RAX, 0						; Zaladuj wstepnie do akumulatora 0 - falsz
							MOV R9, R8						; Zaladuj do R9 szerokosc z R8
							SUB R9, R10						; Odejmij od szerokosci w R9 aktualna kolumne
							JZ FINISH_CHECK_COL_COND		; Jezeli 0  (aktX == wysokosc), koniec
							MOV RAX, 1						; W przeciwnym wypadku prawda, 1 do RAX

FINISH_CHECK_COL_COND:		POP R9							; Posprzataj i zwroc
							RET
CHECK_COL_COND ENDP

; Procedura CHECK_EDGE_COL_COND
; Opis dzialania:
;	Procedura sprawdza warunek: (aktualnyX == 0 || aktualnyX == szerokosc - 1)
; Parametry wejsciowe:
;	R10 -> Aktualny X
;	R8 -> Szerokosc obrazu
; Parametry wyjsciowe:
;	RAX -> 1, jezeli warunek spelniony, 0 jezeli nie
; Niszczone rejestry:
;	BRAK, zawartosc rejestru R9 na ktorym przeprowadzane sa obliczenia odkladana jest na stos
CHECK_EDGE_COL_COND PROC
							PUSH R9							; Zapisz na stosie zawartosc R9
							MOV RAX, 1
							CMP R10, 0						; Porownaj aktualnyX z zerem
							JZ FINISH_CHECK_EDGE_COL_COND
							MOV R9, R8						; Przenies do R9 szerokosc obrazu
							SUB R9, 1						; Odejmij od szerokosci 1
							SUB R9, R10						; Odejmij od szerokosci x
							JZ FINISH_CHECK_EDGE_COL_COND

							MOV RAX, 0
FINISH_CHECK_EDGE_COL_COND:	POP R9							; Posprzataj i zwroc
							RET
CHECK_EDGE_COL_COND ENDP



; Procedura APPLY_FLITER
; Opis dzialania:
;	Procedura iteruje po wejsciowej bitmapie i naklada filtr wykrywajacy krawedzie.
;	Wynik zapisywany jest w bitmapie wyjsciowej.
; Parametry wejsciowe:
;	RCX:		Wskaznik na tablice wejsciowa
;	RDX:		Wskaznik na tablice wyjsciowa
;	R8:			Szerokosc bitmapy
;	R9:			Wysokosc bitmapy
;	[RSP+40]:	Rzadu startowy
;	[RSP+48]:	Rzadu koncowy
; Parametry wyjsciowe:
;	RAX:		W rejestrze RAX znajduje sie informacja zwrotna dla jezyka wysokiego poziomu o bledzie
; Uzywane rejestry (niszcone)
;	RAX
;	RBX
;	RCX
;	RDX
;	R8 -> Szerokosc bitmapy
;	R9 -> Wysokosc bitmapy
;   R10 -> Rzad startorwy
;	R11 -> Rzad koncowy
;	R12 -> Rzad aktualny wejsciowy (currentYIn)
;   R13 -> Rzad aktualny wyjsciowy (currentYOut)
;	R14 -> Indeks rzadu wej		   (currentYIndexIn)
;	R15 -> Indeks rzadu wyj		   (currentYIndexOut)
APPLY_FLITER PROC
							
							MOV R10, [RSP + 40]			; Przeniesienie rzadu startowego do R10
							MOV R12, R10				; Skopiowanie startowego rzadu do R12
							MOV R11, [RSP + 48]			; Przeniesienie rzadu koncowegoo do R11
							XOR R13, R13				; Wyzerowanie aktualnego rzadu wyjsciowego

							; Zaladuj wektory stalych wykrywania krawedzi do rejestrow XMM0, XMM1, XMM2, XMM3
							LEA RAX, [HORIZONTAL_MATRIX_1]
							MOVAPD XMM0, [RAX]
							LEA RAX, [HORIZONTAL_MATRIX_2]
							MOVAPD XMM1, [RAX]
							LEA RAX, [VERTICAL_MATRIX_1]
							MOVAPD XMM2, [RAX]
							LEA RAX, [VERTICAL_MATRIX_2]
							MOVAPD XMM3, [RAX]

							PUSH R15

							; Glowna petla -> Iteracja po rzedach obrazu
MAIN_LOOP:					CALL CHECK_ROW_COND			; if (aktualnyY <= koncowyY) do...
							CMP RAX, 0
							JE FINISH_APPLY_FILTER

							; Sprawdzenie czy pierwszy lub ostatni rzad
							CALL CHECK_EDGE_ROW_COND	; if (aktualny y == 0 || aktualnyY == wysokosc - 1) continue;
							CMP RAX, 1					
							JE CONTINUE_MAIN_LOOP

							PUSH RDX					; Zawartosc rejestru RDX niszczona jest przy mnozeniu, zapisz ja na stos
						
							; R14 -> Indeks rzadu wej = (aktualnyRzad * szerokosc)
							MOV RAX, R12				; Przenies aktualny rzad z R12 do RAX
							MUL R8						; Przemnoz razy szerokosz z R8
							MOV R14, RAX

							; R15 -> Indeks rzadu wyj = ((aktualnyRzad - startowyRzad) * szerokosc)
							MOV RAX, R12				; Przenies aktualny rzad z R12 do RAX
							SUB RAX, R10				; Odejmij rzad poczatkowy
							MUL R8						; Przemnoz razy szerokosc z R8
							MOV R15, RAX

							POP RDX						; Zdejmij ze stosu zawartosc rejestru RDX

							; Petla wewnetrza -> Iteracja po kolumnach obrazu
							; Wymagane: wysokosc (R9), szerokosc (R8), indeks rzadu wej (R14), indeks rzadu wyj (R15)
							; Zwolnienie R10, R11, R12, R13
							PUSH R10
							PUSH R11
							PUSH R12
							PUSH R13

							; R10 -> Aktualny X
							MOV R10, 0
COL_LOOP:					CALL CHECK_COL_COND			; if (aktualnyX < szerokosc) do...
							CMP RAX, 0
							JE FINISH_COL_LOOP

							; Sprawdzenie czy pierwsza lub ostatnia kolumna
							CALL CHECK_EDGE_COL_COND	; if (aktualnyX == 0 || aktualnyX == szer - 1) continue
							CMP RAX, 1
							JE CONTINUE_COL_LOOP

							; R11 -> IndeksXWej = IndeksYWej + x
							MOV R11, R10				; Zaladuj X do R11
							ADD R11, R14				; Dodaj indeksYWej

							; R12 -> IndeksXWyj = IndeksYWyj + x
							MOV R12, R10				; Zaladuj X do R12
							ADD R12, R15				; Dodaj indeksYWej

							; Odloz R15 i R14 (indeksYWej i indeksYWyj) na stos
							PUSH R14					; IndeksYWej
							PUSH R15					; IndeksYWyj
							PUSH R13					; YWyj
							PUSH R12					; IndeksXWyj

							;;; POCZATEK ALGORTYMU
							PUSH R11					; Odloz IndeksXWej na stos



							; XMM0 -> Dane o sasiadujacych pikselach
							; Rzad gorny
							; R12 -> Indeks piksela o jeden rzad wyzej = indeksXWyj - szer
							SUB R11, R8					; Odejmij szerokosc od indeksXWyj

							PINSRB XMM4, BYTE PTR [RCX + R11 - 1], 0		; Zaladuj lewy gorny piksel do XMM4 na 0. pozycje
							PINSRB XMM4, BYTE PTR [RCX + R11], 1			; Zaladuj srodkowy gorny piksel do XMM4 na 1. pozycje
							PINSRB XMM4, BYTE PTR [RCX + R11 + 1], 2		; Zaladuj prawy gorny piksel do XMM4 na 2. pozycje

							; Rzad srodkowy
							ADD R11, R8					; Przesun wskaznik jeden rzad do przodu
							PINSRB XMM4, BYTE PTR [RCX + R11 - 1], 3	; Zaladuj lewy srodkowy piksel do XMM4 na 3. pozycje

							PINSRB XMM5, BYTE PTR [RCX + R11 + 1], 0	; Zaladuj lewy srodkowy piksel do XMM5 na 0. pozycje

							; Rzad dolny
							ADD R11, R8					; Przesun wskaznik jeden rzad do przodu
							PINSRB XMM5, BYTE PTR [RCX + R11 - 1], 1	; Zaladuj lewy dolny piksel do XMM5 na 1. pozycje
							PINSRB XMM5, BYTE PTR [RCX + R11], 2		; Zaladuj srodkowy dolny piksel do XMM5 na 2. pozycje
							PINSRB XMM5, BYTE PTR [RCX + R11 + 1], 3	; Zaladuj prawy dolny piksel do XMM5 na 3. pozycje

							; Skonweruj na 4 x DWORD rejestry XMM4 i XMM5
							PMOVZXBD XMM4, XMM4
							PMOVZXBD XMM5, XMM5

							; XMM6 - working vector for multiplying and horizontal add
							; XMM7 - Suma horyzontalna
							PXOR XMM7, XMM7						; Wyzeruj rejestr sumy horyzontalnej
							
							; Horyzontalna 1
							VPMADDWD XMM6, XMM4, XMM0			; Przemnoz i dodaj parami rejestry XMM4 i XMM0, wynik zapisz w XMM6
							VPHADDD  XMM6, XMM6, XMM6			; Dodaj horyznontalnie zawartosc XMM6
							VPHADDD  XMM6, XMM6, XMM6			; Ponownie dodaj horyzontalnie XMM6 aby otrzymac skumulowana sume
							PADDD XMM7, XMM6					; Dodaj skumulowana sume do wektora sumy horyzontalnej

							; Horyzontalna 2
							VPMADDWD XMM6, XMM5, XMM1			; Przemnoz i dodaj parami rejestry XMM5 i XMM1, wynik zapisz w XMM6
							VPHADDD  XMM6, XMM6, XMM6			; Dodaj horyznontalnie zawartosc XMM6
							VPHADDD  XMM6, XMM6, XMM6			; Ponownie dodaj horyzontalnie XMM6 aby otrzymac skumulowana sume
							PADDD XMM7, XMM6					; Dodaj skumulowana sume do sumy horyzontalnej

							; Vertical 1
							; XMM8 - Suma wertykalna
							PXOR XMM8, XMM8						; Wyzeruj rejestr sumy wertykalnej

							VPMADDWD XMM6, XMM4, XMM2			; Przemnoz i dodaj parami rejestry XMM4 i XMM2, wynik zapisz w XMM6
							VPHADDD XMM6, XMM6, XMM6			; Dodaj horyznontalnie zawartosc XMM6
							VPHADDD XMM6, XMM6, XMM6			; Ponownie dodaj horyzontalnie XMM6 aby otrzymac skumulowana sume
							PADDD XMM8, XMM6					; Dodaj skumulowana sume do wektora sumy wertykalnej

							; Vertical 2
							VPMADDWD XMM6, XMM5, XMM3			; Przemnoz i dodaj parami rejestry XMM5 i XMM3, wynik zapisz w XMM6
							VPHADDD XMM6, XMM6, XMM6			; Dodaj horyznontalnie zawartosc XMM6
							VPHADDD XMM6, XMM6, XMM6			; Ponownie dodaj horyzontalnie XMM6 aby otrzymac skumulowana sume
							PADDD XMM8, XMM6					; Dodaj skumulowana sume do wektora sumy wertykalnej

							PMULLD XMM7, XMM7					; Oblicz kwadrat sumy horyzontalnej
							PMULLD XMM8, XMM8					; Oblicz kwadrat sumy wertykalnej
							PADDD XMM7, XMM8					; Zsumuj kwadraty, wynik zapisz w XMM7

							MOVQ R14, XMM7						; Przenies zawartosc XMM7 do R14
							AND R14, 0000ffffh					; Dla prawidlowego odczytu, zaaplikuj maske

							POP R11						; Zdejmij indeks X wej ze stosu

							CVTSI2SD XMM9, R14			; Skonwertuj sume horyzontalna i zapisz ja do XMM0
							SQRTSD XMM9, XMM9			; Spierwiastkuj sume kwadratow i zapisz do XMM0
							CVTSD2SI R14, XMM9 			; Zapisz wynik piksela w R14 jako liczba calkowita

							; Obetnij do 0 - 255
							SUB R14, 255				; Odejmij 255
							JNS CLAMP_TOP				; Jezeli wynik >= 0 -> Przytnij do 255
							ADD R14, 255				; Dodaj 255
							JS CLAMP_BOTTOM				; Jezeli wynik < 0 -> Przytnij do 0
							JMP WRITE_OUT				

CLAMP_TOP:					MOV R14, 255				; Ustaw wynik na 255
							JMP WRITE_OUT				
CLAMP_BOTTOM:				MOV R14, 0					; Ustaw wynik na 0
							JMP WRITE_OUT


							;;; KONIEC ALGORYTMU
						

WRITE_OUT:					POP R12						; Zdejmij ze stosu indeksXWyj
							MOV [RDX + R12], R14		; Zapisz obliczona wartosc obecnosci krawedzi w tablicy wyjsciowej pod adresem przesunietym o indeks wyjsciowy
							
							; Przywroc rejestry R13, R15, R14
							POP R13
							POP R15
							POP R14

CONTINUE_COL_LOOP:			ADD R10, 1					; Inkrementacja X
							JMP COL_LOOP				; Powrot do petli iterujacej po kolumnach

FINISH_COL_LOOP:			; Przywroc rejestry R13, R12, R11, R10
							POP R13
							POP R12
							POP R11
							POP R10

CONTINUE_MAIN_LOOP:			ADD R12, 1					; Inkrementacja YWej
							ADD R13, 1					; Inkrementacja YWyj
							JMP MAIN_LOOP				; Powrot do petli glownej

FINISH_APPLY_FILTER:		POP R15						; Przywrocenie rejestu R15
							XOR RAX, RAX				; Wyzeruj akumulator na znak braku wystapienia bledu
							RET							; Zwrocenie
APPLY_FLITER ENDP

END
