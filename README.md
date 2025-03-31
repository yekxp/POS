# Softvérová dokumentácia

## Celková charakteristika architektúry

Architektúra POS systému je navrhnutá tak, aby umožňovala efektívnu komunikáciu medzi klientmi a backendovými službami. Systém integruje moderné komponenty založené na mikroslužbách s existujúcou staršou monolitickou infraštruktúrou prostredníctvom centralizovanej API Gateway. Systém podporuje cloudové aj lokálne nasadenie, čím poskytuje flexibilitu pre rôzne obchodné potreby.

### Základné komponenty

- **Brána API (Azure API Managment)** 
Funguje ako centrálny bod, ktorý zabezpečuje overovanie, riadenie prístupu a presmerovanie požiadaviek. Podporuje aj skladanie požiadaviek, preklad protokolov a ukladanie do medzipamäte.

- **Azure Relay (komunikačný most)** 
Zabezpečuje bezpečnú komunikáciu v reálnom čase medzi cloudom a lokálnymi POS servermi. Eliminuje potrebu verejných IP adries alebo VPN.

- **POS Backend** 
Simuluje tradičný POS systém zodpovedný za základné maloobchodné funkcie, ako je spracovanie objednáviek alebo pridávanie produktov.

- **Backoffice** 
Pozostáva z modulárnych služieb zodpovedných za správu používateľov a analytické dáta. Služby sú nasadené prostredníctvom Azure App Services a nezávisle škálovateľné.

- **MongoDB** 
Databáza NoSQL založená na dokumentoch, ktorú používajú všetky služby na ukladanie a načítanie údajov.

### Schéma
