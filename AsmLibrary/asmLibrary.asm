; Wykrywanie Krawedzi operatorem Sobela
; Dawid Kacza

; Wersja 0.2 (19.11.2023)
;	Przekazywanie parametrow z programu wysokiego poziomu
; Wersja 0.3 (26.11.2023)
;	Podstawowa iteracja po bitmapie
; Wersja 0.4 (2.12.2023)
;	Implementacja konwersji na czarno-bialy kolor
; Wersja 1.0 (13.01.2023)
;	Implementacja wlasciwej konwersji

; ZMIENNE
.DATA
BYTES_PER_PIXEL QWORD 3
PIXEL_MASK QWORD 00ffffffh
RED_MASK QWORD 00ff0000h
GREEN_MASK QWORD 0000ff00h
BLUE_MASK QWORD 000000ffh
NTSC_RED REAL8 0.299
NTSC_GREEN REAL8 0.587
NTSC_BLUE REAL8 0.114

; Macierz przeksztalcenia wertykalna
;  -1 0 1
;  -2 0 2
;  -1 0 1
TOP_LEFT_V QWORD -1
TOP_CENTER_V QWORD 0
TOP_RIGHT_V QWORD 1
CENTER_LEFT_V QWORD -2
CENTER_CENTER_V QWORD 0
CENTER_RIGHT_V QWORD 2
BOTTOM_LEFT_V QWORD-1
BOTTOM_CENTER_V QWORD 0
BOTTOM_RIGHT_V QWORD 1

; Macierz przeksztalcenia horyzontalna
;   1  2  1
;   0  0  0
;  -1 -2 -1
TOP_LEFT_H QWORD 1
TOP_CENTER_H QWORD 2
TOP_RIGHT_H QWORD 1
CENTER_LEFT_H QWORD 0
CENTER_CENTER_H QWORD 0
CENTER_RIGHT_H QWORD 0
BOTTOM_LEFT_H QWORD -1
BOTTOM_CENTER_H QWORD -2
BOTTOM_RIGHT_H QWORD -1

.CODE

; Procedura GET_GRAYSCALE
; Opis dzialania:
;	Procedura przelicza wartosci R, G, B na skale szarosci
; Parametry wejsciowe:
;	RCX -> Wskaznik na poczatek tablicy wejsciowej
;	R11 -> Indeks piksela w tablicy
; Parametry wyjsciowe:
;	RAX -> Wartosc Szarosci
; Uzywane rejestry 
;	R8
; Niszczone rejestry
;	BRAK, zawartosc rejestu R8 zapisywana jest na stosie
GET_GRAYSCALE PROC
							PUSH R8
							PUSH R9

							; R8 -> Finalna suma
							XOR R8, R8					; Wyzeruj R8

							; Piksel przechowywany jest pod nastepujacym adresem: [RCX + R11]

							; CZERWONY
							MOV RAX, [RCX + R11]		; Zaladuj pixel do RAX
							AND RAX, RED_MASK			; Zaaplikuj maske by dostac czerwony kolor piksela
							SHR RAX, 16					; Wyrownaj bitowo
							CVTSI2SD XMM0, RAX			; Skonwertuj na zmiennoprzecinkowa w XMM0
							MOVSD XMM1, NTSC_RED		; Zaladuj do XMM1 stala koloru czerwonego
							MULSD XMM0, XMM1			; Przemnoz wartosc czerwieni piksela razy stala
							CVTSD2SI RAX, XMM0			; Zaladuj wynik do RAX (skonwertowany na liczbe calkowita)
							ADD R8, RAX					; Dodaj do finalnej sumy

							; ZIELONY
							MOV RAX, [RCX + R11]		; Zaladuj pixel do RAX
							AND RAX, GREEN_MASK			; Zaaplikuj maske by dostac zielony kolor piksela
							SHR RAX, 8					; Wyrownaj bitowo
							CVTSI2SD XMM0, RAX			; Skonwertuj na zmiennoprzecinkowa w XMM0
							MOVSD XMM1, NTSC_GREEN		; Zaladuj do XMM1 stala koloru zielonego
							MULSD XMM0, XMM1			; Przemnoz wartosc zieleni piksela razy stala
							CVTSD2SI RAX, XMM0			; Zaladuj wynik do RAX (skonwertowany na liczbe calkowita)
							ADD R8, RAX					; Dodaj do finalnej sumy

							; NIEBIESKI
							MOV RAX, [RCX + R11]		; Zaladuj pixel do RAX
							AND RAX, BLUE_MASK			; Zaaplikuj maske by dostac niebieski kolor piksela
							CVTSI2SD XMM0, RAX			; Skonwertuj na zmiennoprzecinkowa w XMM0
							MOVSD XMM1, NTSC_BLUE		; Zaladuj do XMM1 stala koloru niebieskiego
							MULSD XMM0, XMM1			; Przemnoz wartosc blekitu piksela razy stala
							CVTSD2SI RAX, XMM0			; Zaladuj wynik do RAX (skonwertowany na liczbe calkowita)
							ADD R8, RAX					; Dodaj do finalnej sumy


							MOV RAX, R8					; Zaladuj finalna sume do RAX

							POP R9
							POP R8
							RET
GET_GRAYSCALE ENDP

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

							PUSH R15

							; Glowna petla -> Iteracja po rzedach obrazu
MAIN_LOOP:					CALL CHECK_ROW_COND			; if (aktualnyY <= koncowyY) do...
							CMP RAX, 0
							JE FINISH_APPLY_FILTER

							; Sprawdzenie czy pierwszy lub ostatni rzad
							CALL CHECK_EDGE_ROW_COND	; if (aktualny y == 0 || aktualnyY == wysokosc - 1) continue;
							CMP RAX, 1					; TODO FIX
							JE CONTINUE_MAIN_LOOP

							PUSH RDX
						
							; R14 -> Indeks rzadu wej = (aktualnyRzad * szerokosc * bajtyNaPixel)
							MOV RAX, R12				; Przenies aktualny rzad z R12 do RAX
							MUL R8						; Przemnoz razy szerokosz z R8
							MUL BYTES_PER_PIXEL			; Przemnoz razy bajty na pixel
							MOV R14, RAX

							; R15 -> Indeks rzadu wyj = ((aktualnyRzad - startowyRzad) * szerokosc * bajtyNaPixel)
							MOV RAX, R12				; Przenies aktualny rzad z R12 do RAX
							SUB RAX, R10				; Odejmij rzad poczatkowy
							MUL R8						; Przemnoz razy szerokosc z R8
							MUL BYTES_PER_PIXEL			; Przemnoz razy bajty na pixel
							MOV R15, RAX

							POP RDX

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

							PUSH RDX

							; R11 -> IndeksXWej = IndeksYWej + (x * BYTES_PER_PIXEL)
							MOV RAX, R10				; Zaladuj X do RAX
							MUL BYTES_PER_PIXEL			; Przemnoz razy BYTES_PER_PIXEL
							ADD RAX, R14				; Dodaj indeksYWej
							MOV R11, RAX

							; R12 -> IndeksXWyj = IndeksYWyj + (x * BYTES_PER_PIXEL)
							MOV RAX, R10				; Zaladuj X do RAX
							MUL BYTES_PER_PIXEL			; Przemnoz razy BYTES_PER_PIXEL
							ADD RAX, R15				; Dodaj indeksYWej
							MOV R12, RAX

							POP RDX

							; Odloz R15 i R14 (indeksYWej i indeksYWyj) na stos
							PUSH R14
							PUSH R15
							PUSH R13
							; Odloz indeks wyjsciowy na stos
							PUSH R12

							; R14 -> Suma Operatora Horyzontalnego = 0
							XOR R14, R14
							; R15 -> Suma Operatora Wertykalnego = 0
							XOR R15, R15

							PUSH RDX

							; Rzad gorny
							; R12 -> Indeks piksela o jeden rzad wyzej = indeksXWyj - (szer * BYTES_PER_PIXEL)
							MOV RAX, R8
							MUL BYTES_PER_PIXEL
							MOV R12, R11				; Przenies indeksXWyj do R12
							SUB R12, RAX				; Odejmij (BYTES_PER_PIXEL * szerokosc) od indeksXWyj

							; Lewy gorny piksel
							SUB R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL TOP_LEFT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL TOP_LEFT_V
							ADD R15, RAX

							; Srodkowy gorny piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL TOP_CENTER_H
							ADD R14, RAX
							MOV RAX, R13
							MUL TOP_CENTER_V
							ADD R15, RAX

							; Prawy gorny piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL TOP_RIGHT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL TOP_RIGHT_V
							ADD R15, RAX



							; Rzad Srodkowy
							; R12 -> Indeks aktualnego piksela (do poprzedniej wartosci nalezy dodac width*BYTES_PER_PIXEL)
							MOV RAX, R8
							MUL BYTES_PER_PIXEL
							ADD R12, RAX

							; Lewy srodkowy piksel
							SUB R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL CENTER_LEFT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL CENTER_LEFT_V
							ADD R15, RAX

							; Srodkowy srodkowy piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL CENTER_CENTER_H
							ADD R14, RAX
							MOV RAX, R13
							MUL CENTER_CENTER_V
							ADD R15, RAX

							; Prawy srodkowy piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL CENTER_RIGHT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL CENTER_RIGHT_V
							ADD R15, RAX

							; Rzad Dolny
							; R12 -> Indeks aktualnego piksela (do poprzedniej wartosci nalezy dodac width*BYTES_PER_PIXEL)
							MOV RAX, R8
							MUL BYTES_PER_PIXEL
							ADD R12, RAX

							; Lewy dolny piksel
							SUB R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL BOTTOM_LEFT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL BOTTOM_LEFT_V
							ADD R15, RAX

							; Srodkowy dolny piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL BOTTOM_CENTER_H
							ADD R14, RAX
							MOV RAX, R13
							MUL BOTTOM_CENTER_V
							ADD R15, RAX

							; Prawy dolny piksel
							ADD R12, BYTES_PER_PIXEL
							PUSH R11
							MOV R11, R12
							CALL GET_GRAYSCALE
							POP R11
							; RAX przechowuje wartosc szarosci pixela
							MOV R13, RAX
							MUL BOTTOM_RIGHT_H
							ADD R14, RAX
							MOV RAX, R13
							MUL BOTTOM_RIGHT_V
							ADD R15, RAX

							POP RDX


							CVTSI2SD XMM0, R14			; Skonwertuj sume horyzontalna i zapisz ja do XMM0
							CVTSI2SD XMM1, R15			; Skonwertuj sume wertykalna i zapisz ja do XMM1
							MULSD XMM0, XMM0
							MULSD XMM1, XMM1
							ADDSD XMM0, XMM1
							SQRTSD XMM0, XMM0
							CVTSD2SI R14, XMM0 			; Zapisz wynik piksela w R14

							; Obetnij do 0 - 255
							SUB R14, 255
							JNS CLAMP_TOP
							ADD R14, 255
							JS CLAMP_BOTTOM
							JMP WRITE_OUT

CLAMP_TOP:					MOV R14, 255
							JMP WRITE_OUT
CLAMP_BOTTOM:				MOV R14, 0
							JMP WRITE_OUT
						


WRITE_OUT:					POP R12						; Zdejmij ze stosu indeksXWyj
							MOV [RDX + R12], R14
							MOV [RDX + R12 + 1], R14
							MOV [RDX + R12 + 2], R14
							




							; R14 -> Skala szarosci pixela
							;CALL GET_GRAYSCALE			; Wpisanie do RAX wartosci szarosci piksela
							;MOV R14, RAX				; Przeniesienie szarosci do R14

							; RAX -> Adres pamieci wyjsciowej
							;MOV RAX, RDX				; Zaladowanie adresu tablicy wyjsciowej do RAX
							;ADD RAX, R12				; Dodanie indeksu aktualnego piksela do wskaznika tablicy wyjsciowej
							;MOV [RAX], R14				; Zaladowanie wyliczonej wartosci do tablicy pod wskazany przez RAX adres
							;MOV [RAX + 1], R14			; Zaladowanie wyliczonej wartosci do tablicy pod wskazany przez RAX adres
							;MOV [RAX + 2], R14			; Zaladowanie wyliczonej wartosci do tablicy pod wskazany przez RAX adres


							
							POP R13
							POP R15
							POP R14
							



CONTINUE_COL_LOOP:			ADD R10, 1					; Inkrementacja X
							JMP COL_LOOP



FINISH_COL_LOOP:			POP R13
							POP R12
							POP R11
							POP R10


CONTINUE_MAIN_LOOP:			ADD R12, 1					; Inkrementacja YWej
							ADD R13, 1					; Inkrementacja YWyj
							JMP MAIN_LOOP



FINISH_APPLY_FILTER:		POP R15
							RET							; Zwrocenie
APPLY_FLITER ENDP


END
