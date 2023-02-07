using System;
using System.IO;
using System.Collections.Generic;

Dictionary<string, int> flags = new Dictionary<string, int>();
int currentInst = 0;

if (args.Length == 0)
{
    Console.WriteLine("Você precisa passar um parâmetro para o arquivo a ser montado.");
    return;
}

var filePath = args[0];

if (!File.Exists(filePath))
{
    Console.WriteLine("O arquivo especifiado não existe.");
    return;
}

StreamWriter writer = null;
StreamReader reader = null;
try
{
    reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        line = flagFinder(line);
    }

    writer = new StreamWriter("memory");
    writer.WriteLine("v2.0 raw");
    reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        line = processLine(line);
        if (line != string.Empty)
        {
            writer.Write(line);
            writer.Write(" ");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("O seguinte erro ocorreu durante o processo:");
    Console.WriteLine(ex.Message);
}
finally
{
    reader.Close();
    writer.Close();
}

void flagFinder(string line)
{
    ;
}

string processLine(string line)
{

    string[] operations = new string[] {"and", "or", "not", "xor", "add", "sub", "mul", "div", "neg", "lsh", "rsh", "inc", "dec"};

    byte inst = 0;
    byte par0 = 0;
    byte par1 = 0;
    byte par2 = 0;

    Boolean flagger = false;

    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (string part in parts)
    {
        Console.WriteLine(part);
    }
    Console.WriteLine(" ----- FIM DA LINHA ----- ");


    if (parts[0].Contains(':'))
    {
        flagger = true;
        flags.Add(parts[0].Replace(":", string.Empty), currentInst);
    }

    if (Array.IndexOf(operations, parts[0]) != -1)
    {
        inst = dec("0001");
        par0 = (byte)(Array.IndexOf(operations, parts[0]));
        par1 = Convert.ToByte(parts[1].Replace("$", string.Empty).Replace(",", string.Empty));
        if (parts.Length > 2)
        {
            par2 = Convert.ToByte(parts[2].Replace("$", string.Empty).Replace(",", string.Empty));
        }
    }

    byte[] inValue8bit = new byte[2];
    byte[] inValue12bit = new byte[3];

    switch (parts[0])
    {
        case "mov":
            if(isReg(parts[2]))
            {
                inst = dec("0011");
                par0 = 0;
                par1 = Convert.ToByte(parts[1].Replace("$", string.Empty).Replace(",", string.Empty));
                par2 = Convert.ToByte(parts[2].Replace("$", string.Empty).Replace(",", string.Empty));
            }
            else
            {
                inValue8bit = convertValue8bitsV2(parts[2]);
                inst = dec("0010");
                par0 = Convert.ToByte(parts[1].Replace("$", string.Empty).Replace(",", string.Empty));
                par1 = inValue8bit[0];
                par2 = inValue8bit[1];
            }
            break;

        case "cmp":
            if(isReg(parts[2]))
            {
                inst = dec("0100");
                par0 = 0;
                par1 = Convert.ToByte(parts[1].Replace("$", string.Empty).Replace(",", string.Empty));
                par2 = Convert.ToByte(parts[2].Replace("$", string.Empty).Replace(",", string.Empty));
            }
            else
            {
                inValue8bit = convertValue8bitsV2(parts[2]);
                inst = dec("0101");
                par0 = Convert.ToByte(parts[1].Replace("$", string.Empty).Replace(",", string.Empty));
                par1 = inValue8bit[0];
                par2 = inValue8bit[1];
            }
            break;

        case "jump":
            inValue12bit = convertValue12bitsV2(parts[1]);
            inst = dec("0110");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break;

        case "je":
            inValue12bit = convertValue12bitsV2((flags[parts[1]]).ToString());
            inst = dec("0111");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break;

        case "jne":
            inValue12bit = convertValue12bitsV2((flags[parts[1]]).ToString());
            inst = dec("1000");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break;

        case "jg":
            inValue12bit = convertValue12bitsV2((flags[parts[1]]).ToString());
            inst = dec("1000");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break;

        case "jge":
            inValue12bit = convertValue12bitsV2((flags[parts[1]]).ToString());
            inst = dec("1010");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break; 

        case "jz":
            inValue12bit = convertValue12bitsV2((flags[parts[1]]).ToString());
            inst = dec("1011");
            par0 = inValue12bit[0];
            par1 = inValue12bit[1];
            par2 = inValue12bit[2];
            break; 

        default:
            //  Console.WriteLine("Unknown instruction");
            break;
    }

    if (flagger)
    {
        return string.Empty;
    }

    currentInst++;

    byte[] opCode = new byte[4] {inst, par0, par1, par2};

    return toHex(opCode);
}

byte dec(string binary)
{
    int binaryNumber = int.Parse(binary);
    int decimalValue = 0;
    int base1 = 1;

    while (binaryNumber > 0)
    {
        int reminder = binaryNumber % 10;
        binaryNumber = binaryNumber / 10;
        decimalValue += reminder * base1;
        base1 = base1 * 2;
    }
    return (byte)decimalValue;
}

byte[] convertValue4bits(int value)
{
    byte[] binary = new byte[4];
    for (int i = 0; i < 4; i++)
    {
        binary[3 - i] = (byte)((value & (1 << i)) >> i);
    }

    return binary;
}

byte[] convertValue8bitsV2(string value)
{
    int number = Convert.ToInt32(value);

    byte[] binary = new byte[8];

    for (int i = 0; i < 8; i++)
    {
        binary[7 - i] = (byte)((number & (1 << i)) >> i);
    }

    byte[] decimalBytes = new byte[2];
    decimalBytes[0] = (byte)(binary[0] * 8 + binary[1] * 4 + binary[2] * 2 + binary[3] * 1);
    decimalBytes[1] = (byte)(binary[4] * 8 + binary[5] * 4 + binary[6] * 2 + binary[7] * 1);

    return decimalBytes;

}

byte[] convertValue12bitsV2(string value)
{
    int number = Convert.ToInt32(value);

    byte[] binary = new byte[12];

    for (int i = 0; i < 12; i++)
    {
        binary[11 - i] = (byte)((number & (1 << i)) >> i);
    }

    byte[] decimalBytes = new byte[2];
    decimalBytes[0] = (byte)(binary[0] * 8 + binary[1] * 4 + binary[2] * 2 + binary[3] * 1);
    decimalBytes[1] = (byte)(binary[4] * 8 + binary[5] * 4 + binary[6] * 2 + binary[7] * 1);
    decimalBytes[2] = (byte)(binary[8] * 8 + binary[9] * 4 + binary[10] * 2 + binary[11] * 1);

    return decimalBytes;

}


Boolean isReg(string value)
{
    if (value.IndexOf("$") != -1)
    {
        return true;
    }
    return false;
}

string toHex(byte[] code)
{
    string hex1 = code[0].ToString("X");
    string hex2 = code[1].ToString("X");
    string hex3 = code[2].ToString("X");
    string hex4 = code[3].ToString("X");

    string hexString = hex1 + hex2 + hex3 + hex4;

    return hexString;
}