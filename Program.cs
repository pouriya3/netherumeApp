using System;
using System.Text;
using System.Threading.Tasks;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.Signer;
using Nethereum.HdWallet;


class Encoder
{
    static long TimeStamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public static string EncodeAndHashForBetTables(string userAddress, string competitor, string tableAddress, uint betAmountInTether, uint nonce)
    {
        var abiEncode = new ABIEncode();

        BigInteger formattedTime = new BigInteger((long)Math.Floor(TimeStamp / 30.0));

        BigInteger betAmountWithDecimals = new BigInteger(betAmountInTether) * 1000000;


        userAddress = userAddress.Substring(2);
        competitor = competitor.Substring(2);
        tableAddress = tableAddress.Substring(2);

        var encodedValue = abiEncode.GetABIEncoded([
            new ABIValue("address", userAddress),
            new ABIValue("address", competitor),
            new ABIValue("address", tableAddress),
            new ABIValue("uint256", betAmountWithDecimals),
            new ABIValue("uint64", nonce),
            new ABIValue("uint256", formattedTime),
            ]).ToHex();

        Console.WriteLine("Encoded Value: " + encodedValue);

        // encodedValue = encodedValue.Substring(24);
        var hash = new Sha3Keccack().CalculateHash(encodedValue.HexToByteArray());

        return hash.ToHex();
    }
    public static string EncodeAndHashForTournament(string walletAddress, string TournametAddress, uint betAmountInTether, uint nonce)
    {
        var abiEncode = new ABIEncode();

        BigInteger betAmountWithDecimals = new BigInteger(betAmountInTether) * 1000000;

        walletAddress = walletAddress.Substring(2);
        TournametAddress = TournametAddress.Substring(2);

        var encodedValue = abiEncode.GetABIEncoded([
            new ABIValue("address", walletAddress),
            new ABIValue("address", TournametAddress),
            new ABIValue("address", betAmountWithDecimals),new ABIValue("uint16", nonce)
         ]).ToHex();
        Console.WriteLine("Encoded Value: " + encodedValue);
        // encodedValue = encodedValue.Substring(24);
        var hash = new Sha3Keccack().CalculateHash(encodedValue.HexToByteArray());
        Console.WriteLine("Encoded hash Value: " + hash);

        return hash.ToHex();
    }
    public static string hashForJwt()
    {
        string hash = "0xdbaaa4203ec767d6c9ac1cb78fc705bcd58b5ea0aded824b0cb0000000000000";

        var hexTime = new BigInteger(TimeStamp);
        var timeByteArray = hexTime.ToByteArray();
        var hexTimeBytes = timeByteArray.ToHex();
        hash = hash.Substring(0, hash.Length - hexTimeBytes.Length);
        hash += hexTimeBytes;
        return hash;

    }
}
class SignHash
{
    public static string Sign(string hash, string words)
    {
        // var hashByteArray = HexByteConvertorExtensions.HexToByteArray(hash);
        var wallet = new Wallet(words, null);
        var account = wallet.GetAccount(0);

        var signer = new EthereumMessageSigner();
        // var bytes = hashByteArray;

        return signer.Sign(hash.HexToByteArray(), new EthECKey(account.PrivateKey));
    }
}


class Program
{

    static void Main(string[] args)
    {

        var mnemonic = "..."; // 12 word mnemonic
        var wallet = new Wallet(mnemonic, null);

        var account = wallet.GetAccount(0);

        // // var signer = new EthereumMessageSigner();
        Console.WriteLine("Address: " + account.Address);
        // // account.Address should be smart Account address

        string hash = Encoder.EncodeAndHashForBetTables(account.Address, "0x8cb7051a62689241366Dc685625036cd7f81cd06",
         "0x092eb9d061167FC931805a36ec8621E805553A7F", 1, 3);
        Console.WriteLine("hash for betTables: " + hash);

        var signature = SignHash.Sign(hash, mnemonic);

        Console.WriteLine("Signed Hash for betTables: " + signature);

        // string hash = Encoder.EncodeAndHashForTournament(account.Address, account.Address, 1, 1);
        // Console.WriteLine("hash for betTables: " + hash);


        // // signature should send for betTable process
        // var signature = SignHash.Sign(hash, mnemonic);

        // var jwtHash = Encoder.hashForJwt();
        // Console.WriteLine("JWT hash  :" + jwtHash);
        // var signatureforJwt = SignHash.Sign(jwtHash, mnemonic);

        // Console.WriteLine("JWT signature:   " + signatureforJwt);

    }
}
