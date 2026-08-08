using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds.Utils
{
    public static class SoundEventUtils
    {
        public static uint GenerateSoundHash(string soundEventName)
        {
            return MurmurHash2(soundEventName, 0x53524332);
        }

        internal static uint MurmurHash2(string input, uint seed)
        {
            byte[] a2 = System.Text.Encoding.ASCII.GetBytes(input.ToLowerInvariant());
            long v4 = a2.Length;
            uint v6 = (uint)(v4 ^ seed);
            int offset = 0;
            if (v4 >= 4)
            {
                uint v7 = (uint)(v4 >> 2);
                v4 -= 4 * v7;
                while (v7 > 0)
                {
                    uint v8 = 1540483477 * BitConverter.ToUInt32(a2, offset);
                    v6 = (1540483477 * (v8 ^ (v8 >> 24))) ^ (1540483477 * v6);
                    offset += 4;
                    v7--;
                }
            }
            int v9 = (int)v4 - 1;
            if (v9 != 0)
            {
                int v10 = v9 - 1;
                if (v10 != 0)
                {
                    if (v10 != 1)
                    {
                        v6 ^= v6 >> 13;
                        return (1540483477 * v6) ^ ((1540483477 * v6) >> 15);
                    }
                    v6 ^= (uint)(a2[offset + 2] << 16);
                }
                v6 ^= (uint)(a2[offset + 1] << 8);
            }
            v6 = 1540483477 * (v6 ^ a2[offset]);
            v6 ^= v6 >> 13;
            return (1540483477 * v6) ^ ((1540483477 * v6) >> 15);
        }
    }

    public class SoundParams
    {
        private const uint FieldNameHashSeed = 0xB5D9F700;
        private const byte FloatFieldType = 4;
        private readonly List<(string name, byte type, byte[] data)> _parameters = [];

        public uint Guid { get; set; }
        public RecipientFilter? Recipients { get; set; }

        public SoundParams Volume(float volume)
        {
            _parameters.Add(("volume", FloatFieldType, BitConverter.GetBytes(volume)));
            return this;
        }

        public void Send()
        {
            UserMessage message = UserMessage.FromId(210);
            message.SetUInt("soundevent_guid", Guid);
            message.SetBytes("packed_params", BuildPackedParams());
            if (Recipients != null)
            {
                message.Recipients = Recipients;
            }

            message.Send();
        }

        private byte[] BuildPackedParams()
        {
            List<byte> result = [];

            foreach ((string name, byte type, byte[] data) in _parameters)
            {
                uint paramNameHash = SoundEventUtils.MurmurHash2(name, FieldNameHashSeed);
                result.AddRange(BitConverter.GetBytes(paramNameHash));
                result.Add(type);
                result.Add((byte)data.Length);
                result.Add(0);
                result.AddRange(data);
            }

            return [.. result];
        }
    }
}
