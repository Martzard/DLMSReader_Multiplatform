using System;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System.Text;
using System.Text.Json;

namespace DLMS_Diplomka03.Shared.Components.Models;

class MyGXDLMSObjectCollection : GXDLMSObjectCollection
    {

        public MyGXDLMSObjectCollection(GXDLMSObjectCollection existingObjects)
        {
            foreach (var obj in existingObjects)
            {
                this.Add(obj);
            }
        }

        public string CreateXml()
        {
            GXXmlWriterSettings settings = new GXXmlWriterSettings();
            settings.Values = true;
            settings.IgnoreDefaultValues = true;
            settings.IgnoreDescription = false;

            using MemoryStream ms = new MemoryStream();

            bool flag = false;
            if (settings != null)
            {
                flag = settings.IgnoreDescription;
                _ = settings.OmitXmlDeclaration;
            }

            int num = 0;
            if (settings != null)
            {
                num = settings.Index;
            }

            int num2 = 2;
            using (IEnumerator<GXDLMSObject> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    GXDLMSObject current = enumerator.Current;
                    if (current is GXDLMSAssociationLogicalName)
                    {
                        num2 = current.Version;
                        break;
                    }
                }
            }

            using GXXmlWriter gXXmlWriter = new GXXmlWriter(ms, settings);
            if (!flag)
            {
                gXXmlWriter.WriteStartDocument();
            }

            gXXmlWriter.WriteStartElement("Objects", 0);
            using (IEnumerator<GXDLMSObject> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    GXDLMSObject current2 = enumerator.Current;
                    if (!(current2 is IGXDLMSBase))
                    {
                        continue;
                    }

                    if (num < 2)
                    {
                        if (settings == null || !settings.Old)
                        {
                            gXXmlWriter.WriteStartElement("GXDLMS" + current2.ObjectType, 0);
                        }
                        else
                        {
                            gXXmlWriter.WriteStartElement("Object", 0);
                            gXXmlWriter.WriteAttributeString("Type", ((int)current2.ObjectType).ToString(), 0);
                        }
                    }

                    if (num == 0)
                    {
                        if (current2.ShortName != 0)
                        {
                            gXXmlWriter.WriteElementString("SN17", current2.ShortName, 0);
                        }

                        gXXmlWriter.WriteElementString("LN", current2.LogicalName, 0);
                        if (current2.Version != 0)
                        {
                            gXXmlWriter.WriteElementString("Version", current2.Version, 0);
                            if (current2 is GXDLMSAssociationLogicalName)
                            {
                                num2 = current2.Version;
                            }
                        }

                        if (!flag && !string.IsNullOrEmpty(current2.Description))
                        {
                            gXXmlWriter.WriteElementString("Description", current2.Description, 0);
                        }

                        if (num2 < 3)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int i = 1; i != (current2 as IGXDLMSBase).GetAttributeCount() + 1; i++)
                            {
                                stringBuilder.Append(((int)current2.GetAccess(i)).ToString());
                            }

                            gXXmlWriter.WriteElementString("Access", stringBuilder.ToString(), 0);
                            stringBuilder.Length = 0;
                            for (int j = 1; j != (current2 as IGXDLMSBase).GetMethodCount() + 1; j++)
                            {
                                stringBuilder.Append(((int)current2.GetMethodAccess(j)).ToString());
                            }

                            gXXmlWriter.WriteElementString("MethodAccess", stringBuilder.ToString(), 0);
                        }
                        else
                        {
                            StringBuilder stringBuilder2 = new StringBuilder();
                            for (int k = 1; k != (current2 as IGXDLMSBase).GetAttributeCount() + 1; k++)
                            {
                                stringBuilder2.Append(((int)((AccessMode3)32768 | current2.GetAccess3(k))).ToString("X"));
                            }

                            gXXmlWriter.WriteElementString("Access3", stringBuilder2.ToString(), 0);
                            stringBuilder2.Length = 0;
                            for (int l = 1; l != (current2 as IGXDLMSBase).GetMethodCount() + 1; l++)
                            {
                                stringBuilder2.Append(((int)((MethodAccessMode3)32768 | current2.GetMethodAccess3(l))).ToString("X"));
                            }

                            gXXmlWriter.WriteElementString("MethodAccess3", stringBuilder2.ToString(), 0);
                        }
                    }

                    if (settings == null || settings.Values)
                    {
                        (current2 as IGXDLMSBase).Save(gXXmlWriter);
                    }

                    if (num < 2)
                    {
                        gXXmlWriter.WriteEndElement();
                    }
                }
            }

            gXXmlWriter.WriteEndElement();
            if (!flag)
            {
                gXXmlWriter.WriteEndDocument();
            }

            // Ujistíme se, že vše je zapsáno do MemoryStreamu.
            gXXmlWriter.Flush();

            // Převedeme MemoryStream na řetězec (kódování dle GXXmlWriter, pravděpodobně UTF-8).
            // Pokud GXXmlWriter explicitně nastavuje jiné kódování, upravte to i zde.
            return Encoding.UTF8.GetString(ms.ToArray());
        }


        public void SaveJson(string filename, GXXmlWriterSettings settings)
        {
            // Nastavení
            bool ignoreDescription = settings?.IgnoreDescription ?? false;
            int index = settings?.Index ?? 0;

            // Zjistíme verzi (num2 v původním kódu) z GXDLMSAssociationLogicalName.
            int associationVersion = 2;
            using (IEnumerator<GXDLMSObject> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is GXDLMSAssociationLogicalName aln)
                    {
                        associationVersion = aln.Version;
                        break;
                    }
                }
            }

            // Seznam objektů, které budeme serializovat do JSON.
            List<object> objectList = new List<object>();

            // Projedeme všechny GXDLMSObject v kolekci.
            using (IEnumerator<GXDLMSObject> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    GXDLMSObject current = enumerator.Current;
                    if (current is not IGXDLMSBase)
                    {
                        // Pokud objekt není IGXDLMSBase, přeskočíme.
                        continue;
                    }

                    // Připravíme slovník (nebo anonymní objekt) s daty,
                    // která chceme ukládat do JSON.
                    var jsonObject = new Dictionary<string, object>();

                    // Původní kód podmíněně zapisoval "GXDLMS<ObjectType>" nebo "Object Type=...".
                    // V JSON si to můžete zjednodušit. Např. ukládat vždy "ObjectType":
                    if (index < 2)
                    {
                        // Pokud settings?.Old == true, v XML se používalo "Object" s atributem Type.
                        // Můžeme zachovat podobný koncept i v JSON.
                        if (settings == null || !settings.Old)
                        {
                            jsonObject["ObjectType"] = "GXDLMS" + current.ObjectType;
                        }
                        else
                        {
                            jsonObject["Type"] = (int)current.ObjectType;
                        }
                    }

                    // Původní kód: if (num == 0) => zápis SN, LN, Version, Description...
                    // Tady 'num' == 'index'.
                    if (index == 0)
                    {
                        // SN (Short Name)
                        if (current.ShortName != 0)
                        {
                            jsonObject["SN"] = current.ShortName;
                        }

                        // LN (Logical Name)
                        jsonObject["LN"] = current.LogicalName;

                        // Version
                        if (current.Version != 0)
                        {
                            jsonObject["Version"] = current.Version;

                            // Pokud je to AssociationLogicalName, nastavíme "associationVersion".
                            if (current is GXDLMSAssociationLogicalName)
                            {
                                associationVersion = current.Version;
                            }
                        }

                        // Description (pokud není ignorováno)
                        if (!ignoreDescription && !string.IsNullOrEmpty(current.Description))
                        {
                            jsonObject["Description"] = current.Description;
                        }

                        // Access / Access3, MethodAccess / MethodAccess3 podle verze
                        if (associationVersion < 3)
                        {
                            // Access
                            StringBuilder sbAccess = new StringBuilder();
                            for (int i = 1; i <= (current as IGXDLMSBase).GetAttributeCount(); i++)
                            {
                                sbAccess.Append((int)current.GetAccess(i));
                            }
                            jsonObject["Access"] = sbAccess.ToString();

                            // MethodAccess
                            StringBuilder sbMethodAccess = new StringBuilder();
                            for (int i = 1; i <= (current as IGXDLMSBase).GetMethodCount(); i++)
                            {
                                sbMethodAccess.Append((int)current.GetMethodAccess(i));
                            }
                            jsonObject["MethodAccess"] = sbMethodAccess.ToString();
                        }
                        else
                        {
                            // Access3
                            StringBuilder sbAccess3 = new StringBuilder();
                            for (int i = 1; i <= (current as IGXDLMSBase).GetAttributeCount(); i++)
                            {
                                // V původním kódu: (AccessMode3)32768 | current.GetAccess3(i) => bitová kombinace
                                int val = (int)((AccessMode3)32768 | current.GetAccess3(i));
                                sbAccess3.Append(val.ToString("X"));
                            }
                            jsonObject["Access3"] = sbAccess3.ToString();

                            // MethodAccess3
                            StringBuilder sbMethodAccess3 = new StringBuilder();
                            for (int i = 1; i <= (current as IGXDLMSBase).GetMethodCount(); i++)
                            {
                                int val = (int)((MethodAccessMode3)32768 | current.GetMethodAccess3(i));
                                sbMethodAccess3.Append(val.ToString("X"));
                            }
                            jsonObject["MethodAccess3"] = sbMethodAccess3.ToString();
                        }
                    }

                    // Původní kód: if (settings == null || settings.Values) => volání (current as IGXDLMSBase).Save(...)
                    // Zde byste měli buďto mít vlastní metodu na JSON (třeba (current as IGXDLMSBase).SaveJson(...)),
                    // anebo sem ručně doplnit vlastnosti, které se jinak ukládaly do XML.
                    if (settings == null || settings.Values)
                    {
                        // Příklad volání hypotetické metody, která vrátí Dictionary s extra daty:
                        // var extraData = (current as IGXDLMSBase).GetJsonData();
                        // foreach(var kvp in extraData) 
                        // {
                        //     jsonObject[kvp.Key] = kvp.Value;
                        // }

                        // Pro ukázku:
                        jsonObject["ValueData"] = $"Data from {current.GetType().Name}";
                    }

                    // Přidáme výsledný objekt do seznamu
                    objectList.Add(jsonObject);
                }
            }

            // Výsledná JSON struktura: { "Objects": [ {...}, {...}, ... ] }
            var finalObject = new
            {
                Objects = objectList
            };

            // Serializace do JSON řetězce s odsazením:
            string jsonString = JsonSerializer.Serialize(
                finalObject,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            );

            // Zápis do souboru
            File.WriteAllText(filename, jsonString);
        }
    }
