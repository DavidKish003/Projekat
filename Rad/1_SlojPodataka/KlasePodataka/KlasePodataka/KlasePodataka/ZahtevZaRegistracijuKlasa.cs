using System;
using System.ComponentModel.DataAnnotations;
namespace KlasePodataka { public class ZahtevZaRegistracijuKlasa {
 public int ZahtevID {get;set;}
 [Required(ErrorMessage="Unesite ime i prezime")] public string ImePrezime {get;set;}
 [Required,EmailAddress(ErrorMessage="Unesite ispravnu e-mail adresu")] public string Email {get;set;}
 [Required(ErrorMessage="Unesite telefon")] public string KontaktTelefon {get;set;}
 [Required(ErrorMessage="Unesite naziv konkursa")] public string NazivKonkursa {get;set;}
 [Required(ErrorMessage="Izaberite stepen obrazovanja")] public string StepenObrazovanja {get;set;}
 [Range(0,50,ErrorMessage="Godine iskustva moraju biti između 0 i 50")] public int GodineIskustva {get;set;}
 public string MotivacionoPismo {get;set;}
 public DateTime DatumPodnosenja {get;set;}
 [Required] public DateTime RokKonkursa {get;set;}
 public string StatusZahteva {get;set;}
 public bool IspunjavaOsnovneUslove {get;set;}
 }}