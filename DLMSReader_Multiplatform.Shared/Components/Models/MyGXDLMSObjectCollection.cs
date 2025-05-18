using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System.Text;

namespace DLMSReader_Multiplatform.Shared.Components.Models;

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
            settings.IgnoreDefaultValues = false;
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

            gXXmlWriter.Flush();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
