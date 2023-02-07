using System;
using System.IO;

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
    writer = new StreamWriter("memory");
    writer.WriteLine("v2.0 raw");
    reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        line = processLine(line);
        writer.Write(line);
        writer.Write(" ");
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

string processLine(string line)
{

    byte[] opCode = new byte[16];

    byte[] inst = new byte[4];
    byte[] par0 = new byte[4];
    byte[] par1 = new byte[4];
    byte[] par2 = new byte[4];

    string[] parts = line.Split(' ');

    int index = 0;
    
    switch (parts[0]){
        case "mov":
            byte[] r = convertValue4bits(Convert.ToInt32(parts[1].Remove('$').Remove(',')));
            byte[] c = convertValue8bits(Convert.ToInt32(parts[2]));
            opCode = new byte[16] {0, 0, 1, 0, r[0], r[1], r[2], r[3], c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]};
            foreach (var item in opCode)
            {
                Console.WriteLine(item);
            }    
            break;
        case "inc":
            //Console.WriteLine("Increment register: " + parts[1]);
            break;
        case "cmp":
            //Console.WriteLine("Compare registers: " + parts[1] + " and " + parts[2]);
            break;
        case "je":
            //Console.WriteLine("Jump to label: " + parts[1]);
            break;
        case "jump":
           // Console.WriteLine("Unconditional jump to label: " + parts[1]);
            break;
        case "end:":
            //Console.WriteLine("End of program");
            break;
        default:
          //  Console.WriteLine("Unknown instruction");
            break;
    }    

    return toHex(opCode);
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

byte[] convertValue8bits(int value)
{
    byte[] binary = new byte[8]; 
    for (int i = 0; i < 8; i++)
    {
        binary[7 - i] = (byte)((value & (1 << i)) >> i);
    }

    return binary;
}


string toHex(byte[] code)
{
    string hex = "";

    for (int i = 0; i < code.Length; i += 4)
    {
        int value = code[i] << 3 | code[i + 1] << 2 | code[i + 2] << 1 | code[i + 3];
        hex += value.ToString("X1");
    }

    Console.WriteLine(hex);
    return hex;
}