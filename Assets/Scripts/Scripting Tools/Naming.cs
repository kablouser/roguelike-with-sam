using System.Text;

public static class Naming
{
    public static string SpaceOutCamelCase(string camelCase)
    {
        StringBuilder stringBuilder = new StringBuilder();

        for(int i = 0; i < camelCase.Length; i++)
        {
            if(0 < i && char.IsUpper(camelCase, i))
                stringBuilder.Append(' ');
            stringBuilder.Append(camelCase[i]);
        }

        return stringBuilder.ToString();
    }
}
