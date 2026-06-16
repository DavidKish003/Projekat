(function () {
    "use strict";

    document.addEventListener(
        "DOMContentLoaded",
        function () {
            inicijalizujValidacijuPrijave();
        }
    );

    /**
     * Pokreće validaciju forme za dodavanje ili izmenu prijave.
     *
     * @returns {void}
     */
    function inicijalizujValidacijuPrijave() {
        var pronadjenaForma =
            document.getElementById("FormaPrijave");

        if (!(pronadjenaForma instanceof HTMLFormElement)) {
            return;
        }

        var forma =
            pronadjenaForma;

        var dozvoliProsliRok =
            forma.getAttribute("data-dozvoli-prosli-rok") === "true";

        var polja =
            forma.querySelectorAll(".kontrola-polja");

        for (
            var indeks = 0;
            indeks < polja.length;
            indeks++
        ) {
            var polje =
                polja[indeks];

            if (
                polje instanceof HTMLInputElement
                ||
                polje instanceof HTMLSelectElement
                ||
                polje instanceof HTMLTextAreaElement
            ) {
                poveziUklanjanjeGreske(polje);
            }
        }

        forma.addEventListener(
            "submit",
            function (dogadjaj) {
                var formaJeIspravna =
                    proveriFormu(
                        dozvoliProsliRok
                    );

                if (!formaJeIspravna) {
                    dogadjaj.preventDefault();
                    dogadjaj.stopPropagation();
                }
            }
        );
    }

    /**
     * @param {boolean} dozvoliProsliRok
     * @returns {boolean}
     */
    function proveriFormu(dozvoliProsliRok) {
        obrisiJsGreske();

        /** @type {string[]} */
        var greske = [];

        /** @type {HTMLElement|null} */
        var prvoNeispravnoPolje =
            null;

        var imePrezime =
            dajInput("ImePrezime");

        var email =
            dajInput("Email");

        var kontaktTelefon =
            dajInput("KontaktTelefon");

        var poslednjaSkola =
            dajInput("PoslednjaSkola");

        var mestoSkole =
            dajInput("MestoSkole");

        var zanimanje =
            dajInput("Zanimanje");

        var stepenObrazovanja =
            dajSelect("StepenObrazovanja");

        var nazivKonkursa =
            dajInput("NazivKonkursa");

        var radnoMesto =
            dajInput("RadnoMesto");

        var godineIskustva =
            dajInput("GodineIskustva");

        var rokKonkursa =
            dajInput("RokKonkursa");

        var motivacionoPismo =
            dajTextarea("MotivacionoPismo");

        if (imePrezime !== null) {
            var vrednostImena =
                imePrezime.value.trim();

            var regexImePrezime =
                /^[A-Za-zČĆŽŠĐčćžšđ]+(?:[ '-][A-Za-zČĆŽŠĐčćžšđ]+)+$/;

            if (vrednostImena === "") {
                postaviGresku(
                    imePrezime,
                    "ImePrezime",
                    "Unesite ime i prezime.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || imePrezime;
            }
            else if (
                vrednostImena.length < 5
                ||
                !regexImePrezime.test(vrednostImena)
            ) {
                postaviGresku(
                    imePrezime,
                    "ImePrezime",
                    "Unesite puno ime i prezime, na primer: Marko Marković.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || imePrezime;
            }
            else if (vrednostImena.length > 100) {
                postaviGresku(
                    imePrezime,
                    "ImePrezime",
                    "Ime i prezime mogu imati najviše 100 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || imePrezime;
            }
            else {
                oznaciKaoIspravno(
                    imePrezime
                );
            }
        }

        if (email !== null) {
            var vrednostEmaila =
                email.value.trim();

            var regexEmail =
                /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;

            if (vrednostEmaila === "") {
                postaviGresku(
                    email,
                    "Email",
                    "Unesite e-mail adresu.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || email;
            }
            else if (!regexEmail.test(vrednostEmaila)) {
                postaviGresku(
                    email,
                    "Email",
                    "Unesite ispravnu e-mail adresu.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || email;
            }
            else if (vrednostEmaila.length > 100) {
                postaviGresku(
                    email,
                    "Email",
                    "E-mail adresa može imati najviše 100 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || email;
            }
            else {
                oznaciKaoIspravno(
                    email
                );
            }
        }

        if (kontaktTelefon !== null) {
            var vrednostTelefona =
                kontaktTelefon.value.trim();

            var normalizovanTelefon =
                vrednostTelefona.replace(
                    /[\s\-\/().]/g,
                    ""
                );

            var regexTelefon =
                /^(?:06\d{7,8}|\+3816\d{7,8})$/;

            if (vrednostTelefona === "") {
                postaviGresku(
                    kontaktTelefon,
                    "KontaktTelefon",
                    "Unesite kontakt telefon.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || kontaktTelefon;
            }
            else if (!regexTelefon.test(normalizovanTelefon)) {
                postaviGresku(
                    kontaktTelefon,
                    "KontaktTelefon",
                    "Telefon mora biti u formatu 06xxxxxxxx ili +3816xxxxxxxx.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || kontaktTelefon;
            }
            else if (vrednostTelefona.length > 20) {
                postaviGresku(
                    kontaktTelefon,
                    "KontaktTelefon",
                    "Kontakt telefon može imati najviše 20 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || kontaktTelefon;
            }
            else {
                oznaciKaoIspravno(
                    kontaktTelefon
                );
            }
        }

        if (poslednjaSkola !== null) {
            var vrednostSkole =
                poslednjaSkola.value.trim();

            if (vrednostSkole === "") {
                postaviGresku(
                    poslednjaSkola,
                    "PoslednjaSkola",
                    "Unesite poslednju završenu školu.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || poslednjaSkola;
            }
            else if (vrednostSkole.length < 3) {
                postaviGresku(
                    poslednjaSkola,
                    "PoslednjaSkola",
                    "Naziv škole mora imati najmanje 3 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || poslednjaSkola;
            }
            else if (vrednostSkole.length > 150) {
                postaviGresku(
                    poslednjaSkola,
                    "PoslednjaSkola",
                    "Naziv škole može imati najviše 150 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || poslednjaSkola;
            }
            else {
                oznaciKaoIspravno(
                    poslednjaSkola
                );
            }
        }

        if (mestoSkole !== null) {
            var vrednostMestaSkole =
                mestoSkole.value.trim();

            if (vrednostMestaSkole.length > 100) {
                postaviGresku(
                    mestoSkole,
                    "MestoSkole",
                    "Mesto škole može imati najviše 100 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || mestoSkole;
            }
            else if (vrednostMestaSkole !== "") {
                oznaciKaoIspravno(
                    mestoSkole
                );
            }
        }

        if (zanimanje !== null) {
            var vrednostZanimanja =
                zanimanje.value.trim();

            if (vrednostZanimanja === "") {
                postaviGresku(
                    zanimanje,
                    "Zanimanje",
                    "Unesite ili izaberite stečeno zanimanje.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || zanimanje;
            }
            else if (vrednostZanimanja.length < 3) {
                postaviGresku(
                    zanimanje,
                    "Zanimanje",
                    "Zanimanje mora imati najmanje 3 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || zanimanje;
            }
            else if (vrednostZanimanja.length > 150) {
                postaviGresku(
                    zanimanje,
                    "Zanimanje",
                    "Zanimanje može imati najviše 150 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || zanimanje;
            }
            else {
                oznaciKaoIspravno(
                    zanimanje
                );
            }
        }

        if (stepenObrazovanja !== null) {
            var vrednostStepena =
                stepenObrazovanja.value.trim();

            if (vrednostStepena === "") {
                postaviGresku(
                    stepenObrazovanja,
                    "StepenObrazovanja",
                    "Izaberite stepen obrazovanja.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || stepenObrazovanja;
            }
            else {
                oznaciKaoIspravno(
                    stepenObrazovanja
                );
            }
        }

        if (nazivKonkursa !== null) {
            var vrednostKonkursa =
                nazivKonkursa.value.trim();

            if (vrednostKonkursa === "") {
                postaviGresku(
                    nazivKonkursa,
                    "NazivKonkursa",
                    "Unesite naziv konkursa.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || nazivKonkursa;
            }
            else if (vrednostKonkursa.length < 3) {
                postaviGresku(
                    nazivKonkursa,
                    "NazivKonkursa",
                    "Naziv konkursa mora imati najmanje 3 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || nazivKonkursa;
            }
            else if (vrednostKonkursa.length > 150) {
                postaviGresku(
                    nazivKonkursa,
                    "NazivKonkursa",
                    "Naziv konkursa može imati najviše 150 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || nazivKonkursa;
            }
            else {
                oznaciKaoIspravno(
                    nazivKonkursa
                );
            }
        }

        if (radnoMesto !== null) {
            var vrednostRadnogMesta =
                radnoMesto.value.trim();

            if (vrednostRadnogMesta === "") {
                postaviGresku(
                    radnoMesto,
                    "RadnoMesto",
                    "Unesite ili izaberite radno mesto.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || radnoMesto;
            }
            else if (vrednostRadnogMesta.length < 3) {
                postaviGresku(
                    radnoMesto,
                    "RadnoMesto",
                    "Radno mesto mora imati najmanje 3 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || radnoMesto;
            }
            else if (vrednostRadnogMesta.length > 150) {
                postaviGresku(
                    radnoMesto,
                    "RadnoMesto",
                    "Radno mesto može imati najviše 150 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || radnoMesto;
            }
            else {
                oznaciKaoIspravno(
                    radnoMesto
                );
            }
        }

        if (godineIskustva !== null) {
            var vrednostGodina =
                godineIskustva.value.trim();

            var brojGodina =
                Number(vrednostGodina);

            if (vrednostGodina === "") {
                postaviGresku(
                    godineIskustva,
                    "GodineIskustva",
                    "Unesite broj godina iskustva.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || godineIskustva;
            }
            else if (
                !Number.isInteger(brojGodina)
                ||
                brojGodina < 0
                ||
                brojGodina > 50
            ) {
                postaviGresku(
                    godineIskustva,
                    "GodineIskustva",
                    "Godine iskustva moraju biti ceo broj između 0 i 50.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || godineIskustva;
            }
            else {
                oznaciKaoIspravno(
                    godineIskustva
                );
            }
        }

        if (rokKonkursa !== null) {
            var vrednostRoka =
                rokKonkursa.value.trim();

            if (vrednostRoka === "") {
                postaviGresku(
                    rokKonkursa,
                    "RokKonkursa",
                    "Unesite rok konkursa.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || rokKonkursa;
            }
            else {
                var izabraniDatum =
                    new Date(
                        vrednostRoka + "T00:00:00"
                    );

                var danas =
                    new Date();

                danas.setHours(
                    0,
                    0,
                    0,
                    0
                );

                if (Number.isNaN(izabraniDatum.getTime())) {
                    postaviGresku(
                        rokKonkursa,
                        "RokKonkursa",
                        "Unesite ispravan datum roka konkursa.",
                        greske
                    );

                    prvoNeispravnoPolje =
                        prvoNeispravnoPolje || rokKonkursa;
                }
                else if (
                    !dozvoliProsliRok
                    &&
                    izabraniDatum < danas
                ) {
                    postaviGresku(
                        rokKonkursa,
                        "RokKonkursa",
                        "Rok konkursa ne može biti datum koji je već prošao.",
                        greske
                    );

                    prvoNeispravnoPolje =
                        prvoNeispravnoPolje || rokKonkursa;
                }
                else {
                    oznaciKaoIspravno(
                        rokKonkursa
                    );
                }
            }
        }

        if (motivacionoPismo !== null) {
            var vrednostPisma =
                motivacionoPismo.value.trim();

            if (vrednostPisma.length > 5000) {
                postaviGresku(
                    motivacionoPismo,
                    "MotivacionoPismo",
                    "Motivaciono pismo može imati najviše 5000 karaktera.",
                    greske
                );

                prvoNeispravnoPolje =
                    prvoNeispravnoPolje || motivacionoPismo;
            }
            else if (vrednostPisma !== "") {
                oznaciKaoIspravno(
                    motivacionoPismo
                );
            }
        }

        if (greske.length > 0) {
            prikaziZajednickeGreske(
                greske
            );

            if (prvoNeispravnoPolje !== null) {
                prvoNeispravnoPolje.focus();

                prvoNeispravnoPolje.scrollIntoView(
                    {
                        behavior: "smooth",
                        block: "center"
                    }
                );
            }

            return false;
        }

        sakrijZajednickeGreske();

        return true;
    }

    /**
     * @param {HTMLInputElement|HTMLSelectElement|HTMLTextAreaElement} polje
     * @param {string} nazivPolja
     * @param {string} poruka
     * @param {string[]} greske
     * @returns {void}
     */
    function postaviGresku(
        polje,
        nazivPolja,
        poruka,
        greske) {

        greske.push(
            poruka
        );

        polje.classList.remove(
            "polje-ispravno"
        );

        polje.classList.add(
            "polje-neispravno"
        );

        polje.setAttribute(
            "aria-invalid",
            "true"
        );

        var pronadjenaPoruka =
            document.querySelector(
                "[data-valmsg-for='" +
                nazivPolja +
                "']"
            );

        if (pronadjenaPoruka instanceof HTMLElement) {
            pronadjenaPoruka.textContent =
                poruka;

            pronadjenaPoruka.classList.remove(
                "field-validation-valid"
            );

            pronadjenaPoruka.classList.add(
                "field-validation-error"
            );

            pronadjenaPoruka.setAttribute(
                "data-js-greska",
                "true"
            );
        }
    }

    /**
     * @param {HTMLInputElement|HTMLSelectElement|HTMLTextAreaElement} polje
     * @returns {void}
     */
    function oznaciKaoIspravno(polje) {
        polje.classList.remove(
            "polje-neispravno"
        );

        polje.classList.add(
            "polje-ispravno"
        );

        polje.setAttribute(
            "aria-invalid",
            "false"
        );
    }

    /**
     * @param {HTMLInputElement|HTMLSelectElement|HTMLTextAreaElement} polje
     * @returns {void}
     */
    function poveziUklanjanjeGreske(polje) {
        var nazivDogadjaja =
            polje instanceof HTMLSelectElement
                ? "change"
                : "input";

        polje.addEventListener(
            nazivDogadjaja,
            function () {
                polje.classList.remove(
                    "polje-neispravno"
                );

                polje.classList.remove(
                    "polje-ispravno"
                );

                polje.removeAttribute(
                    "aria-invalid"
                );

                if (!polje.name) {
                    return;
                }

                var pronadjenaPoruka =
                    document.querySelector(
                        "[data-valmsg-for='" +
                        polje.name +
                        "']"
                    );

                if (
                    pronadjenaPoruka instanceof HTMLElement
                    &&
                    pronadjenaPoruka.getAttribute(
                        "data-js-greska"
                    ) === "true"
                ) {
                    pronadjenaPoruka.textContent =
                        "";

                    pronadjenaPoruka.classList.remove(
                        "field-validation-error"
                    );

                    pronadjenaPoruka.classList.add(
                        "field-validation-valid"
                    );

                    pronadjenaPoruka.removeAttribute(
                        "data-js-greska"
                    );
                }
            }
        );
    }

    /**
     * @returns {void}
     */
    function obrisiJsGreske() {
        var neispravnaPolja =
            document.querySelectorAll(
                ".polje-neispravno, .polje-ispravno"
            );

        for (
            var indeks = 0;
            indeks < neispravnaPolja.length;
            indeks++
        ) {
            var polje =
                neispravnaPolja[indeks];

            if (polje instanceof HTMLElement) {
                polje.classList.remove(
                    "polje-neispravno"
                );

                polje.classList.remove(
                    "polje-ispravno"
                );

                polje.removeAttribute(
                    "aria-invalid"
                );
            }
        }

        var jsPoruke =
            document.querySelectorAll(
                "[data-js-greska='true']"
            );

        for (
            var indeksPoruke = 0;
            indeksPoruke < jsPoruke.length;
            indeksPoruke++
        ) {
            var poruka =
                jsPoruke[indeksPoruke];

            if (poruka instanceof HTMLElement) {
                poruka.textContent =
                    "";

                poruka.classList.remove(
                    "field-validation-error"
                );

                poruka.classList.add(
                    "field-validation-valid"
                );

                poruka.removeAttribute(
                    "data-js-greska"
                );
            }
        }

        sakrijZajednickeGreske();
    }

    /**
     * @param {string[]} greske
     * @returns {void}
     */
    function prikaziZajednickeGreske(greske) {
        var pronadjeniKontejner =
            document.getElementById("jsGreske");

        if (!(pronadjeniKontejner instanceof HTMLElement)) {
            return;
        }

        var html =
            "<strong>Ispravite sledeće greške:</strong><ul>";

        for (
            var indeks = 0;
            indeks < greske.length;
            indeks++
        ) {
            html +=
                "<li>" +
                kodirajHtml(greske[indeks]) +
                "</li>";
        }

        html +=
            "</ul>";

        pronadjeniKontejner.innerHTML =
            html;

        pronadjeniKontejner.classList.add(
            "prikazana"
        );

        pronadjeniKontejner.style.display =
            "block";
    }

    /**
     * @returns {void}
     */
    function sakrijZajednickeGreske() {
        var pronadjeniKontejner =
            document.getElementById("jsGreske");

        if (!(pronadjeniKontejner instanceof HTMLElement)) {
            return;
        }

        pronadjeniKontejner.innerHTML =
            "";

        pronadjeniKontejner.classList.remove(
            "prikazana"
        );

        pronadjeniKontejner.style.display =
            "none";
    }

    /**
     * @param {string} id
     * @returns {HTMLInputElement|null}
     */
    function dajInput(id) {
        var element =
            document.getElementById(id);

        return element instanceof HTMLInputElement
            ? element
            : null;
    }

    /**
     * @param {string} id
     * @returns {HTMLSelectElement|null}
     */
    function dajSelect(id) {
        var element =
            document.getElementById(id);

        return element instanceof HTMLSelectElement
            ? element
            : null;
    }

    /**
     * @param {string} id
     * @returns {HTMLTextAreaElement|null}
     */
    function dajTextarea(id) {
        var element =
            document.getElementById(id);

        return element instanceof HTMLTextAreaElement
            ? element
            : null;
    }

    /**
     * @param {string} vrednost
     * @returns {string}
     */
    function kodirajHtml(vrednost) {
        var element =
            document.createElement("div");

        element.textContent =
            vrednost;

        return element.innerHTML;
    }
})();