# Pavlovic Damjan - Kompresija podataka

Projekat 1
## Algoritmi kompresije

Shannon-Fano je algoritam za kompresiju podataka koji koristi frekvenciju simbola da izgradi binarno stablo kodiranja. Simboli sa većom frekvencijom dobijaju kraće kodove, što omogućava efikasnu kompresiju za podatke sa poznatom statistikom.


Huffmanov algoritam za kompresiju koji gradi binarno stablo na osnovu frekvencije simbola, dodjeljujući kraće kodove češćim simbolima. Koristi prioritetni red za izgradnju stabla i rezultuje optimalnim prefiks kodovima bez gubitka podataka.


LZ77 je algoritam za kompresiju bez gubitaka koji traži ponavljanja u prozoru podataka i zamenjuje ih pokazivačima na prethodne sekvence. Koristi klizni prozor i dužinu podudaranja za efikasnu kompresiju repetitivnih podataka.


LZW (Lempel-Ziv-Welch) je algoritam za kompresiju koji gradi rečnik sekvenci tokom procesa kodiranja. Počinje sa pojedinačnim simbolima i dodaje nove sekvence u rečnik, zamenjujući ih kodovima za bržu kompresiju.

### 1. Bajt.cs
Klasa `Bajt` predstavlja jedan bajt iz ulaznog fajla, sa svojom vrednošću, verovatnoćom pojavljivanja i kodom dodeljenim tokom kompresije.

### 2. UcitaniBajt.cs
Klasa `UcitaniBajt` koristi se pri dekodiranju po istom principu kao i `Bajt`, ali bez verovatnoće.

### 3. Entropija.cs
Klasa `Entropija` računa entropiju ulaznog fajla.

### 4. ShannonFano.cs
Klasa `ShannonFano` implementira Shannon-Fano algoritam:
- `Enkodiraj`: Rekurzivno dodeljuje kodove bajtovima na osnovu verovatnoće.
- `Kod`: Pretvara ulazne podatke u niz bitova koristeći dodeljene kodove.
- `SacuvajUFajl`: Upisuje mapu kodova i kodovane podatke u fajl.

### 5. Huffman.cs
Klasa `Huffman` implementira Huffmanov algoritam:
- `kreirajStablo`: Pravi Huffmanovo stablo od bajtova.
- `dodeliKodove`: Dodeljuje kodove bajtovima na osnovu stabla.
- `Kodiraj`: Koduje podatke koristeći mapu kodova.
- `SacuvajUFajl`: Upisuje mapu kodova i kodovane podatke u fajl, uključuje padding informaciju.

### 6. LZ77.cs
Klasa `LZ77` implementira LZ77 algoritam:
- `Kompresuj`: Kompresuje podatke u listu tuple-ova (offset, length, next byte).
- `Serialize`/`Deserialize`: Pretvara kompresovane podatke u bajt niz i obrnuto.
- `SacuvajUFajl`/`UcitajIzFajla`: Upisuje/čita podatke iz fajla.
- `Dekompresuj`: Rekonstruiše originalne podatke iz kompresovanih.

### 7. LZW.cs
Klasa `LZW` implementira LZW algoritam:
- `Kompresuj`: Kompresuje podatke u listu indeksa.
- `Serialize`/`Deserialize`: Pretvara kompresovane podatke u bajt niz i obrnuto.
- `SacuvajUFajl`/`UcitajIzFajla`: Upisuje/čita podatke iz fajla.
- `Dekompresuj`: Rekonstruiše originalne podatke iz kompresovanih.

### 8. Decode.cs
Klasa `Decode` služi za dekodiranje kompresovanih fajlova:
- `UcitaniBajt`: Učitava mapu kodova iz fajla.
- `DajOstatak`: Vraća binarni niz podataka iz fajla.
- `ProcitajPaddingBitova`: Čita padding informaciju iz fajla.
- `UkloniCifre`: Uklanja padding bitove iz binarnog niza.
- `Dekodiraj`: Pretvara binarni niz nazad u bajtove koristeći mapu kodova.
- `Uporedi`: Poredi originalni i dekodirani fajl.
- `SacuvajDekodiran`: Upisuje dekodirane podatke u fajl.

### 9. StepenKompresije.cs
Klasa `StepenKompresije` računa stepen kompresije za svaki algoritam, poredeći veličinu ulaznog i izlaznog fajla.

### 10. Program.cs
Glavni program:
- Računa entropiju ulaznog fajla.
- Pokreće kompresiju i dekodiranje za svaki algoritam.
- Poredi rezultate dekodiranja sa originalom.
- Prikazuje stepen kompresije i procenat uštede za svaki algoritam.

## 11. Pokretanje
Pokrenite projekat u Visual Studio-u. Ulazni fajl je `ulazVisokaEntropija.bin`. Rezultati kompresije, dekodiranja i stepeni kompresije se prikazuju u konzoli.

## 12. Rezultati
Postoje dva ulazna fajla sa različitom entropijom:
	- `ulazVisokaEntropija.bin`: Visoka entropija, manja kompresija.
	- `ulazNiskaEntropija.bin`: Niska entropija, veća kompresija.

Fajlovi su generisani online alatom za generisanje binarnih fajlova sa zadatim sablonima

Nakon pokretanja programa za oba fajla, dobijamo sledece rezultate:
### za `ulazVisokaEntropija.bin`:
	Stepeni kompresije:
	Shannon-Fano: 0.998x (usteda: -0.2%)
	Huffman:      0.9985x (usteda: -0.15%)
	LZ77:         0.3274x (usteda: -205.44%)
	LZW:          0.4914x (usteda: -103.5%)
### za `ulazNiskaEntropija.bin`:
	Stepeni kompresije:
	Shannon-Fano: 7.9005x (usteda: 87.34%)
	Huffman:      7.9012x (usteda: 87.34%)
	LZ77:         1.7985x (usteda: 44.4%)
	LZW:          185.3053x (usteda: 99.46%)
	



---