# Notes Serialize

Celé řešení je postaveno na třídě `RootDescriptor<T>`. Ta sama o sobě plní funkci Serializeru. Metody `GetSomeTypeDescriptor()` vrací instance `RootDescriptor<T>` specializované na konkrétní typy pak slouží jako konfigurace Serializeru pro konkrétní datový typ (v tomto případě `Person`).

## `RootDescriptor<T>`

Instance třídy `RootDescriptor<T>` specializovaná na konkrétní typ disponuje polem delegátu `FunctionsDescribingThis`, jež obsahuje delegáty na metody obstarávající XML string reprezenzaci jednotlivých fieldů, jimiž disponuje typ, popisovaný touto instancí `RootDescriptor<T>`. Metoda `Serialize`:

```cs
public void Serialize(TextWriter writer, T instance)
		{
			StringBuilder builder = new StringBuilder();
			Serialize(instance, typeof(T).Name, builder);
			writer.Write(builder.ToString());
		}
```

slouží pouze jako wrapper, který metodě `public void Serialize(T instance, string instanceName, StringBuilder builder)` předává `StringBuilder`.  Využití `StringBuilderu` mi v tomto případě dávalo smysl, neboť můžeme potenciálně chtít přes Serializer získat XML reprezentaci rozsáhlého datového typu (obsahujícího hodně fieldů, více úrovní zanoření) a vytvoření takového XML bude vyžadovat mnoho konkatencí stringů.

Vlastní metoda provádějící serializaci:

```csharp
public void Serialize(T instance, string instanceName, StringBuilder builder)
		{
			if (this.FuctionsDescribingThis.Length == 0)
			{
				//simple value
				builder.Append($"<{instanceName}>{instance}</{instanceName}>\n");
			}
			else
			{
				//compound value
				builder.Append($"<{instanceName}>\n");
				foreach (var GetStringRep in this.FuctionsDescribingThis)
				{
                    //function hidden in FunctionDescribingThis call serialize()
                    //hidden recursive call
					GetStringRep(instance, builder); 
				}
				builder.Append($"</{instanceName}>\n");
			}
		}
	}
```

Instance `RootDescriptor<T>` pro jednoduché typy, tedy takové, co v sobě neobsahují vnořené fieldy, jsou vytvořeny s prázdným polem `FunctionsDescribingThis`. První větev `if` v `Serialize()` tedy vytváří XML záznam pro jednoduchý field a zároveň tvoří base case rekurze.

Pro složený typ jsou v `else` větvi postupně zavolány všechny delegáty, přidávající do `StringBuilderu` XML reprezentace jednotlivých fieldů tohoto složeného typu.

## GetTypeDescriptor methody

Tyto metody vrací novou instanci `RootDescriptor<T>` specializovanou na konkrétní typ. Pro jednoduché typy s s prázdným polem `FunctionsDescribingThis` je implementace přímočará, což přispívá k snadné rozšiřitelnosti o nové podporované jednoduché typy. 

```cs
static RootDescriptor<string> GetStringDescriptor()
		{
			return new RootDescriptor<string>();
		}
```

Pro složené typy je ve `GetDescriptor()` metodě třeba vytvořit a naplnit příslušné pole delegátů. Metody, na něž budou delegáti ukazovat, jsou naiplementovány přímo při vytváření instancí delegáta jako lambda funkce. Každý vytvářený delegát je příslušný jednomu fieldu třídy, pro niž konstruujeme descriptor (jednoduchému či vnořenému fieldu). V těle lambda funkce nejprve vytvoříme descriptor pro příslušný field a následně na něm zavoláme metodu `Serialize()` s konkrétní instancí daného filedu. Jelikož jsou tyto lambda funkce volány přes delegáty přímo z metody `Serialize()`, volání `Serialize()` v tělech lambda funkcí je rekurzivním voláním a umožňuje snadno vypsat rekurzivní strukturu dané třídy. Každý delegát tedy poskytuje XML reprezentaci celého podstromu pod fieldem, jemuž je příslušný. Zde spoléhám na to, že  datové typy, ne něž bude Selializer používán, nebudou obsahovat tolik úrovní zanoření, aby došlo k přetečení stacku v důsledku rekurzivních volání non-tail recursion. Tento předpoklad mi ale přijde rozumný.

Je důležité si uvědomit, že ač lambda fcím a delegátům slibujeme, že při zavolání obdrží instanci fieldu, který popisují (a při volání ze `Serialize()` ji také obdrží), samotné vytváření descriptorů je na konkrétní instanci nezávislé.  



