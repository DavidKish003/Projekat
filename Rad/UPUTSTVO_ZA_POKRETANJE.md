# Sistem za prijavu kandidata i obradu konkursa za posao

Projekat je prilagođen iz originalnog primera registracije vozila. Nazivi postojećih Visual Studio projekata i nekih klasa ostavljeni su radi lakšeg učitavanja starih referenci, ali poslovni domen, baza, prikazi i pravilo odnose se na konkurse za posao.

## Pokretanje
1. U SQL Server Management Studio pokrenuti `1_SlojPodataka/BazaPodataka/KOMPLETNA BAZA PODATAKA/BazaPodataka.txt`.
2. Zatim pokrenuti `1_SlojPodataka/BazaPodataka/STORED PROCEDURE/SP.txt`.
3. Build redom: KlasePodataka, PoslovnaLogika, PrezentacionaLogika, servis i MVC.
4. Otvoriti `4_PrezentacioniSloj/KorisnickiInterfejs/RegistracijaVozilaMVC.sln` i pokrenuti IIS Express.
5. Prijava: korisničko ime `admin`, lozinka `admin` (ili `hr` / `hr`).

## Poslovno pravilo
Ako kandidat nije dostavio CV ili je rok konkursa istekao, prijava dobija status Odbijena. U suprotnom dobija status Primljena.
