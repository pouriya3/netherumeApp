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
    static long timestamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public static string EncodeAndHashForBetTables(string address, uint betAmountInTether, bool isTournamnet)
    {
        var abiEncode = new ABIEncode();

        BigInteger formattedTime = new BigInteger((long)Math.Floor(timestamp / (isTournamnet ? 1000.0 : 20.0)));

        BigInteger betAmountWithDecimals = new BigInteger(betAmountInTether) * 1000000;
        if (address.StartsWith("0x"))
        {
            address = address.Substring(2);
        }

        var encodedValue = abiEncode.GetABIEncoded([new ABIValue("address", address), new ABIValue("uint256", betAmountWithDecimals), new ABIValue("uint256", formattedTime)]).ToHex();
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

        var hexTime = new BigInteger(timestamp);
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

        var mnemonic = "flock rude kingdom almost bike attitude buyer initial skull anxiety decade also";

        var wallet = new Wallet(mnemonic, null);

        var account = wallet.GetAccount(0);

        // var signer = new EthereumMessageSigner();
        Console.WriteLine("Address: " + account.Address);
        // account.Address should be smart Account address

        string hash = Encoder.EncodeAndHashForTournament(account.Address,account.Address ,1,1);
        Console.WriteLine("hash for betTables: " + hash);


        // signature should send for betTable process
        var signature = SignHash.Sign(hash, mnemonic);

        Console.WriteLine("Signed Hash for betTables: " + signature);

        // var jwtHash = Encoder.hashForJwt();
        // Console.WriteLine("JWT hash  :" + jwtHash);
        // var signatureforJwt = SignHash.Sign(jwtHash, mnemonic);

        // Console.WriteLine("JWT signature:   " + signatureforJwt);

    }
}
