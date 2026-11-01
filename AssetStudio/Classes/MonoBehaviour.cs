namespace AssetStudio
{
    public class MonoBehaviour : Behaviour
    {
        public PPtr<MonoScript> m_Script;
        public string m_Name;

        public MonoBehaviour() { }

        public MonoBehaviour(ObjectReader reader) : base(reader)
        {
            if (reader.serializedType?.m_Type?.m_Nodes?.Count > 0)
            {
                m_Script = new PPtr<MonoScript>(reader);
                m_Name = reader.ReadAlignedString();
                return;
            }

            // Fallback for Unity assets with type-tree disabled / empty schemas.
            // MonoBehaviour payload in this format is still a script pointer followed by a string.
            m_Script = new PPtr<MonoScript>(reader);
            if (version >= (2, 1))
                m_Name = reader.ReadAlignedString();
        }
    }
}
