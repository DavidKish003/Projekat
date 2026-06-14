using System;

namespace KlaseMapiranja
{
    public class MaperKlasa
    {
        public string VratiStatusTekst(
            bool potrebanTehnicki)
        {
            if (potrebanTehnicki)
            {
                return "Kandidat ispunjava osnovne uslove konkursa";
            }

            return "Kandidat ne ispunjava osnovne uslove konkursa";
        }

        public string FormatirajRegistraciju(
            string registracija)
        {
            if (string.IsNullOrWhiteSpace(registracija))
            {
                return string.Empty;
            }

            return registracija.Trim();
        }
    }
}