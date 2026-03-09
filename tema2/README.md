# Gestiune Companie Transporturi - Proiect PIU

Am realizat o aplicație în C# pentru gestionarea activității unei firme de transport, urmărind evidența șoferilor, a mașinilor și a curselor efectuate.

### Ce am implementat:
- **Gestiune Resurse:** Evidența șoferilor și a vehiculelor disponibile în parcul auto.
- **Monitorizare Kilometraj:** Sistem de calcul pentru kilometrii parcurși de fiecare șofer și actualizarea automată a odometrului mașinii.
- **Istoric Trasee:** Salvarea destinațiilor parcurse pentru a păstra un istoric clar al activității.
- **Intervale de Lucru:** Posibilitatea de a aloca un șofer pe o anumită mașină într-un interval stabilit.

### Structura Claselor:
- `Sofer`: Reține datele de identificare, kilometrii totali și lista de trasee.
- `Masina`: Detalii despre vehicul (număr înmatriculare, model, km la bord).
- `IntervalLucru`: Gestionează asocierea dintre un șofer și o mașină.

### Tehnologii:
- Visual Studio 2022
- .NET / C#
- Git pentru versionare