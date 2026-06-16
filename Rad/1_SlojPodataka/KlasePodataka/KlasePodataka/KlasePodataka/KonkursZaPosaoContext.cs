using System.Data.Entity;

namespace KlasePodataka
{
    public class KonkursiZaPosaoContext :
        DbContext
    {
        public KonkursiZaPosaoContext()
            : base("name=KonkursiZaPosaoEF")
        {
            Database.SetInitializer
                <KonkursiZaPosaoContext>(
                    null
                );
        }

        public DbSet<KorisnikKlasa> Korisnici
        {
            get;
            set;
        }
    }
}