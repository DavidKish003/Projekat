(function () {
    "use strict";

    document.addEventListener(
        "DOMContentLoaded",
        function () {
            inicijalizujKombinovanoPolje(
                "Zanimanje",
                "DugmeZanimanja",
                "ListaZanimanja"
            );

            inicijalizujKombinovanoPolje(
                "RadnoMesto",
                "DugmeRadnihMesta",
                "ListaRadnihMesta"
            );
        }
    );

    /**
     * Povezuje tekstualno polje sa dugmetom
     * i padajućom listom predloga.
     *
     * @param {string} idInputa
     * @param {string} idDugmeta
     * @param {string} idListe
     * @returns {void}
     */
    function inicijalizujKombinovanoPolje(
        idInputa,
        idDugmeta,
        idListe) {

        var pronadjeniInput =
            document.getElementById(idInputa);

        var pronadjenoDugme =
            document.getElementById(idDugmeta);

        var pronadjenaLista =
            document.getElementById(idListe);

        if (!(pronadjeniInput instanceof HTMLInputElement)) {
            return;
        }

        if (!(pronadjenoDugme instanceof HTMLButtonElement)) {
            return;
        }

        if (!(pronadjenaLista instanceof HTMLElement)) {
            return;
        }

        var input =
            pronadjeniInput;

        var dugme =
            pronadjenoDugme;

        var lista =
            pronadjenaLista;

        var pronadjeniOmotac =
            input.closest(".kombinovano-polje");

        if (!(pronadjeniOmotac instanceof HTMLElement)) {
            return;
        }

        var omotac =
            pronadjeniOmotac;

        var pronadjeneStavke =
            lista.querySelectorAll(
                ".stavka-predloga"
            );

        /** @type {HTMLButtonElement[]} */
        var stavke = [];

        var indeks;

        for (
            indeks = 0;
            indeks < pronadjeneStavke.length;
            indeks++
        ) {
            var pronadjenaStavka =
                pronadjeneStavke[indeks];

            if (pronadjenaStavka instanceof HTMLButtonElement) {
                stavke.push(
                    pronadjenaStavka
                );
            }
        }

        var pronadjenaPoruka =
            lista.querySelector(
                ".poruka-bez-predloga"
            );

        /** @type {HTMLElement|null} */
        var porukaBezPredloga =
            pronadjenaPoruka instanceof HTMLElement
                ? pronadjenaPoruka
                : null;

        /**
         * @param {string} vrednost
         * @returns {string}
         */
        function normalizujTekst(vrednost) {
            return (vrednost || "")
                .toLocaleLowerCase("sr-RS")
                .trim();
        }

        /**
         * @returns {void}
         */
        function filtrirajPredloge() {
            var trazeniTekst =
                normalizujTekst(
                    input.value
                );

            var brojVidljivih =
                0;

            for (
                var i = 0;
                i < stavke.length;
                i++
            ) {
                var stavka =
                    stavke[i];

                var vrednostStavke =
                    stavka.getAttribute(
                        "data-vrednost"
                    ) || "";

                var normalizovanaVrednost =
                    normalizujTekst(
                        vrednostStavke
                    );

                var trebaPrikazati =
                    trazeniTekst === ""
                    ||
                    normalizovanaVrednost.indexOf(
                        trazeniTekst
                    ) >= 0;

                stavka.hidden =
                    !trebaPrikazati;

                if (trebaPrikazati) {
                    brojVidljivih++;
                }
            }

            if (porukaBezPredloga !== null) {
                porukaBezPredloga.style.display =
                    brojVidljivih === 0
                        ? "block"
                        : "none";
            }
        }

        /**
         * @returns {void}
         */
        function otvoriListu() {
            filtrirajPredloge();

            lista.classList.add(
                "prikazana"
            );

            dugme.setAttribute(
                "aria-expanded",
                "true"
            );
        }

        /**
         * @returns {void}
         */
        function zatvoriListu() {
            lista.classList.remove(
                "prikazana"
            );

            dugme.setAttribute(
                "aria-expanded",
                "false"
            );
        }

        dugme.addEventListener(
            "click",
            function () {
                var listaJePrikazana =
                    lista.classList.contains(
                        "prikazana"
                    );

                if (listaJePrikazana) {
                    zatvoriListu();
                }
                else {
                    otvoriListu();
                    input.focus();
                }
            }
        );

        input.addEventListener(
            "focus",
            function () {
                otvoriListu();
            }
        );

        input.addEventListener(
            "input",
            function () {
                otvoriListu();
            }
        );

        input.addEventListener(
            "keydown",
            function (dogadjaj) {
                if (dogadjaj.key === "Escape") {
                    zatvoriListu();
                }
            }
        );

        /**
         * @param {HTMLButtonElement} stavka
         * @returns {void}
         */
        function poveziStavkuPredloga(stavka) {
            stavka.addEventListener(
                "click",
                function () {
                    var izabranaVrednost =
                        stavka.getAttribute(
                            "data-vrednost"
                        ) || "";

                    input.value =
                        izabranaVrednost;

                    input.dispatchEvent(
                        new Event(
                            "input",
                            {
                                bubbles: true
                            }
                        )
                    );

                    input.dispatchEvent(
                        new Event(
                            "change",
                            {
                                bubbles: true
                            }
                        )
                    );

                    zatvoriListu();
                    input.focus();
                }
            );
        }

        for (
            indeks = 0;
            indeks < stavke.length;
            indeks++
        ) {
            poveziStavkuPredloga(
                stavke[indeks]
            );
        }

        document.addEventListener(
            "click",
            function (dogadjaj) {
                var kliknutiElement =
                    dogadjaj.target;

                if (!(kliknutiElement instanceof Node)) {
                    return;
                }

                if (!omotac.contains(kliknutiElement)) {
                    zatvoriListu();
                }
            }
        );
    }
})();