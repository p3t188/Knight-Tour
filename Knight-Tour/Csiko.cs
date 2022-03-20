using System;
using System.Collections.Generic;

namespace Knight_Tour
{
    internal class Csiko
    {
        private int N = 8;
        //üres mátrix -- MAGA a SakkTábla
        private readonly int[,] tabla;
        //jövőbeni lépések
        private int kovX, kovY;
        //jelenlegi lépések
        private readonly int jelX, jelY;
        //generikus list a mozgásokról
        private readonly List<Lepes> lep;
        //Lehetséges lépések "mátrixa"
        private readonly int[,] lehetsegesLepes = {
            { 2, 1 },
            { 2, -1 },
            { 1, 2 },
            { 1, -2 },
            { -2, 1 },
            { -2, -1 },
            { -1, 2 },
            { -1, -2 }
        };

        public Csiko(int x, int y)
        {
            tabla = new int[N, N]; // 8x8 sakk-mátrix "table" | tábla létrehozása 
            lep = new List<Lepes>(); // Létrehozva a következő mozgás mentéséhze - Generikus újra felhasználható
            tabla.Initialize(); // alapértelmezettre állítja a táblát (0)
            //Addig csinálja a jelenlegi helyzet beállítását, amíg nem talál több lehetőséget ( FindClosedTour());
            do
            {
                jelX = x;
                jelY = y;
            } while (!JoUt());
        }

        //X,Y koordináták ellenőrzése a sakktábla határain belül
        private bool Ervenyes(int x, int y) // Érvényes?
        {
            return ((x >= 0 && y >= 0) && (x < N && y < N)); // Ha bármelyik koordináta nagyobb N méretnél, v. kissebb 0-nál akkor FALSE
        }


        //X,Y koordináta üres-e? Nem volt-e rá lépés!!
        private bool UresEll(int[,] tabla, int x, int y) // Üres?  | Mátrix adott koordinátájú mezőjének ellenőrzése
        {
            return (Ervenyes(x, y)) && (tabla[x, y] == 0); // Ha érvényes lépés IsValid() az adott koordiáta, akkor megvizsgálja hogy az értéke egyenlő-e 0-val a mátrixban!
        }


        //Lehetséges útvonalak számának keresése!!
        private int UjUt(int[,] tabla, int x, int y) // Hány út van, és hány darab üres (avagy lehet rá lépni) ?
        {
            int count = 0;
            for (int i = 0; i < N; ++i)
            {
                if (UresEll(tabla, (x + lehetsegesLepes[i, 0]), (y + lehetsegesLepes[i, 1]))) // Ha érvényes a lépés, és üres a mező --> lehetséges mátrixban lévő lépés --> count++
                {
                    count++;
                }
            }
            return count;
        }

        //Következő lépés megtalálása
        /*
        Kiválasztunk egy véletlen útvonalat a listából aminek minimális "fokozata" van || Először nézze meg hogy a következő lépés üres lenne-e, majd a "fokozatot"
        Ugyanis a Warnsdorff algoritmus során a lehetséges lépések során a legalacsonyabb felé fog lépni a LÓ
        */


        private bool KovLep(int[,] tabla, int x, int y) //x ,y alapján megpróbálja megtalálni a következő lépést
        {
            int minX = -1, c, minSzog = (N + 1), ujX, ujY;
            int start = new Random().Next(N); // Indulási irányok közül véletlenszerűen választ!
            for (int count = 0; count < N; ++count) // hány fok van?
            {
                int i = (start + count) % N; // véletlenszerű választás
                ujX = x + lehetsegesLepes[i, 0]; // aktuális pont + kijelölt pont
                ujY = y + lehetsegesLepes[i, 1]; // aktuális pont + kijelölt pont
                if ((UresEll(tabla, ujX, ujY)) && (c = UjUt(tabla, ujX, ujY)) < minSzog) //Válassza ki a legkevésbé valószínű elérési úttal rendelkező koordinátátát ( Ha érvényes, ha üres, és a lehetséges lépés kissebb mint 9)
                {
                    minX = i;
                    minSzog = c;
                }
            }

            if (minX == -1)
            {
                return false; // ha az érték -1 akkor már használt, vagy nem jó megoldás --> A következő lépés értékének 1-re kell változnia.
            }
            //Szükséges lépés megjelölése
            ujX = x + lehetsegesLepes[minX, 0];
            ujY = y + lehetsegesLepes[minX, 1];
            tabla[ujX, ujY] = tabla[x, y] + 1;
            lep.Add(new Lepes { X = ujX, Y = ujY, Order = tabla[ujX, ujY] }); // hozzáadni a következő lépést a lépések generikus gyűjteményhez úgymond
            kovX = ujX;
            kovY = ujY;
            return true;
        }

        //Két megadott koordináta szomszádságának ellenőrzése
        private bool Szomszedos(int x, int y, int xx, int yy)
        {
            for (int i = 0; i < N; ++i)
                if (((x + lehetsegesLepes[i, 0]) == xx) && ((y + lehetsegesLepes[i, 1]) == yy))
                    return true;

            return false;
        }

        //Megtalálni a megfelelő útvonalat
        // Első funkció fut
        private bool JoUt()
        {
            kovX = jelX;  //a következő pozicíó lesz az új jelenlegi
            kovY = jelY;
            tabla[jelX, jelY] = 1; // Jelenlegi első lépés ( változzon a pozicíó 1-re. Ahol még nem állt a ló ott -1 lesz 
            lep.Add(new Lepes { X = jelX, Y = jelY, Order = 1 }); // Lépés rögzítése


            //64 lépés a befejezésig
            for (int i = 0; i < N * N - 1; ++i)
            {
                if (!KovLep(tabla, kovX, kovY)) // Ha nincs következő lépés, akkor elbukik a ciklus
                {
                    return false;
                }
            }

            //Ellenőrizze
            if (Szomszedos(kovX, kovY, jelX, jelY))
            {
                return false;
            }
            return true;
        }

        public List<Lepes> UjLepes()
        {
            return lep;
        }


        public int[,] UjTabla()
        {
            return tabla;
        }
    }
}
