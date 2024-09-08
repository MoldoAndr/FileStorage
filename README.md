# Transfer Securizat de Fișiere

## Descriere
Sistem client-server de transfer de fișiere implementat în C# folosind Windows Forms pentru interfața client. Oferă capacități securizate de încărcare, descărcare și gestionare a fișierelor. Toate fișierele sunt criptate pentru a asigura confidențialitatea și integritatea datelor. 

## Caracteristici
- Funcționalitate de autentificare și înregistrare securizată
- Încărcarea fișierelor pe server cu criptare automată
- Descărcarea fișierelor de pe server cu decriptare automată
- Ștergerea fișierelor de pe server
- Listarea fișierelor stocate pe server
- Partajarea fișierelor între utilizatori, cu criptare destinată fiecărui utilizator
- Redenumirea fișierelor de pe server
- Vizualizarea conținutului fișierelor de tip text în timp real de pe server
- Criptare TLS pentru toată comunicarea client-server

## Detalii Tehnice
- Limbaj: C#
- Interfață Client: Windows Forms
- Protocol de Rețea: TCP cu TLS
- Autentificare Server: Certificat X509 autosemnat (pentru dezvoltare)
- Criptare Fișiere: 
  - Fișierele sunt criptate folosind o cheie specifică pentru fiecare utilizator.
  - La încărcarea unui fișier, acesta este criptat folosind cheia utilizatorului.
  - La descărcarea unui fișier, acesta este decriptat automat pentru a fi accesibil utilizatorului.
  - Fișierele partajate între utilizatori sunt criptate cu cheia destinatarului, asigurând că doar acesta poate decripta și accesa fișierele respective.
- Bază de date:
  -  SQL Server pentru gestionarea utilizatorilor și hash-urilor parolelor
- Porturile pe care se realizează conexiunea sunt 8888 în cazul serverului mereu, iar în cazul clientului unul alocat dinamic care este stocat în fișier pentru o eventuală analiză a pachetelor.
- Adresa IP a serverului poate fi modificată din fișierul Server/IP.txt de către client

## Note de Securitate
- Implementarea curentă utilizează un certificat autosemnat pentru TLS.
- Criptarea fișierelor adaugă un strat suplimentar de securitate, protejând datele chiar și în cazul în care fișierele sunt interceptate sau accesibile de către terți neautorizați.

## Îmbunătățiri Viitoare
- Îmbunătățirea măsurilor de securitate, inclusiv implementarea unor metode de criptare mai avansate
- Dezvoltarea unei interfețe web pentru accesibilitate mai largă
