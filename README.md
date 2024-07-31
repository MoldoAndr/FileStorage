# Aplicație de Transfer Securizat de Fișiere

## Descriere
Această aplicație este un sistem client-server de transfer de fișiere implementat în C# folosind Windows Forms pentru interfața client. 
Oferă capacități securizate de încărcare, descărcare și gestionare a fișierelor utilizând Transport Layer Security (TLS) pentru comunicare criptată.

## Caracteristici
- Funcționalitate de autentificare și înregistrare securizată
- Încărcarea fișierelor pe server
- Descărcarea fișierelor de pe server
- Ștergerea fișierelor de pe server
- Listarea fișierelor stocate pe server
- Criptare TLS pentru toată comunicarea client-server

## Componente
1. Aplicația Client:
   - InitialForm: Punct de intrare pentru autentificare sau înregistrare
   - LoginForm: Autentificarea utilizatorilor
   - SignupForm: Înregistrarea noilor utilizatori
   - FileForm: Interfața principală pentru operațiunile cu fișiere

2. Aplicația Server:
   - Gestionează conexiunile clienților
   - Administrează autentificarea utilizatorilor
   - Efectuează operațiuni cu fișiere (încărcare, descărcare, ștergere, listare)

## Detalii Tehnice
- Limbaj: C#
- Interfață Client: Windows Forms
- Protocol de Rețea: TCP cu TLS
- Autentificare Server: Certificat X509 autosemnat (pentru dezvoltare)

## Note de Securitate
- Implementarea curentă utilizează un certificat autosemnat pentru TLS.

## Îmbunătățiri Viitoare
- Implementarea partajării de fișiere între utilizatori
- Adăugarea controlului versiunilor pentru fișiere
- Îmbunătățirea măsurilor de securitate (de exemplu, autentificare în doi factori)
- Dezvoltarea unei interfețe web pentru accesibilitate mai largă
