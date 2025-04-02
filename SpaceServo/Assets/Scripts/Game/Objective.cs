using UnityEngine;

[System.Serializable]
public class Objective
{
    [field: SerializeField] public EType Type { get; private set; }
    [field: SerializeField] public float Value { get; private set; }
    [field: SerializeField] public ScriptableObject Config { get; private set; }
    [field: SerializeField] public GameObject TargetObject { get; set; }
    [field: SerializeField] public bool Complete { get; set; }
    [field: SerializeField] public bool Hidded { get; set; }

    public enum EType
    {
        BuildRoom,
        BuildPlaceable,
        HireEmployee,
        Rating,
        MoneyTotal,
        CustomerTotal
    }

    public string ObjectiveText
    {
        get
        {
            string output = "";

            switch (Type)
            {
                case EType.BuildRoom:
                    Room roomConfig = (Room)Config;
                    if (roomConfig != null)
                        output += "Build a " + roomConfig.Name;
                    break;

                case EType.BuildPlaceable:
                    Placeable placeableConfig = (Placeable)Config;
                    if (placeableConfig != null)
                        output += "Build a " + placeableConfig.Name;
                    break;

                case EType.HireEmployee:
                    Placeable placeableEmployeeConfig = (Placeable)Config;
                    if (placeableEmployeeConfig != null)
                        output += "Hire a staff member for " + placeableEmployeeConfig.Name;
                    break;

                case EType.Rating:
                    output += Value/10 + " out of " + Station.Rating.MAX_RATING/10 + " star raiting";
                    break;

                case EType.MoneyTotal:
                    output += "Have $" + Value;
                    break;

                case EType.CustomerTotal:
                    output += "Serve " + Value.ToString("f0") + " customers";
                    break;
            }

            return output;
        }
    }
}
