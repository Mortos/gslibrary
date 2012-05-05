using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gslibrary
{
    public interface IGameMessage
    {
        int StructSize();
    }
    public class GameMessage : BasicTypes.BaseStruct, IGameMessage
    {
        private static readonly Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

        public virtual int StructSize() { return 0; }

        public int Id {get; set;}
        public enum Opcodes {
           Lol=1,
            Al=2,

        }

        public GameMessage(int id)
        {
            this.Id = id;
        }

        public GameMessage()
        {
        }

        public static T Allocate<T>(Opcodes opcode) where T : GameMessage
        {
            if (!gslibrary.Opcodes.tomsg.ContainsKey((int)opcode))
            {
                throw new Exception("Unimplemented message: " + opcode.ToString());
                return null;
            }

            var ctorWithParameterExists = gslibrary.Opcodes.tomsg[(int)opcode].GetConstructor(new[] { typeof(Opcodes) }) != null;
            var msg = (T)Activator.CreateInstance(gslibrary.Opcodes.tomsg[(int)opcode], ctorWithParameterExists ? new object[] { opcode } : null);

            msg.Id = (int)opcode;
            //msg.Consumer = MessageConsumers[opcode];
            return msg;
        }

        public void EncodeMessage(GameBitBuffer buf)
        {

        }

        public static GameMessage ParseMessage(GameBitBuffer buffer)
        {
            int id = buffer.PeekInt(9);
            var msg = Allocate<GameMessage>((Opcodes)id);
            if (msg == null) return null;

            msg.Id = id;
            msg.Parse(buffer);
            return msg;
        }
    }
}
