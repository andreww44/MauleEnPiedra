using UnityEngine;

public class GestorDePetroglifo : MonoBehaviour
{
    public Transform[] slotsPetroglifo;
    void Update()
    {
        if (slotsPetroglifo.Length == 3)
        {
            bool todosOcupados = true;

            foreach (Transform slot in slotsPetroglifo)
            {
                if (slot.childCount == 0)
                {
                    todosOcupados = false;
                    break;
                }
            }

            if (todosOcupados)
            {
                foreach (Transform slot in slotsPetroglifo)
                {
                    foreach (Transform carta in slot)
                    {
                        Destroy(carta.gameObject);
                    }
                }

                Debug.Log("Petroglifo completo");

                this.enabled = false;
            }
        }
    }
}
