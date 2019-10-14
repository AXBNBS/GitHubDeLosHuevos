
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TureganoHouses : MonoBehaviour
{
    [SerializeField] private int housesX, housesZ, housesY, limit;
    [SerializeField] private float differenceX, differenceZ, differenceY;
    [SerializeField] private GameObject house;
    [SerializeField] private Transform transformX, transformXZ, transformZ;
    private Transform[, ,] houses;
    private bool sense;


    // Start is called before the first frame update.
    private void Start ()
    {
        this.transform.position = new Vector3 ((transformX.position.x + transformXZ.position.x) / 2, this.transform.position.y, (transformZ.position.z + transformXZ.position.z) / 2);
        houses = new Transform[housesY, housesZ, housesX];
        sense = true;

        for (int y = -housesY / 2; y <= (+housesY / 2); y += 1) 
        {
            for (int z = -housesZ / 2; z <= (+housesZ / 2); z += 1) 
            {
                for (int x = -housesX / 2; x <= (+housesX / 2); x += 1) 
                {
                    print("hey");
                    houses[y + housesY / 2, z + housesZ / 2, x + housesX / 2] = Instantiate(house, new Vector3 (this.transform.position.x + x * differenceX, this.transform.position.y + y * differenceY, 
                        this.transform.position.z + z * differenceZ), this.transform.rotation).transform;
                    //houses.Add (Instantiate(house, new Vector3 (this.transform.position.x + x * differenceX, this.transform.position.y + y * differenceY, this.transform.position.z + z * differenceZ),
                        //this.transform.rotation).transform);
                }
            }
        }
    }


    // Similar to the regular Update method, only this one is called at fixed intervals.
    private void FixedUpdate ()
    {
        for (int y = 0; y < houses.Length; y += 1) 
        {
            for (int z = 0; z < houses.GetLength (y); z += 1) 
            {
                //for
            }
        }
        /*if (sense == false)
        {
            for (int i = 0; i < houses.Count; i += 1)
            {
                if (i % 2 == 0)
                {
                    houses[i].transform.Translate (-this.transform.up / 10);
                }
                else
                {
                    houses[i].transform.Translate (this.transform.up / 10);
                }
            }
            this.transform.Translate (this.transform.up / 10);
        }
        else 
        {
            for (int i = 0; i < houses.Count; i += 1)
            {
                if (i % 2 == 0)
                {
                    houses[i].transform.Translate (this.transform.up / 10);
                }
                else
                {
                    houses[i].transform.Translate (-this.transform.up / 10);
                }
            }
            this.transform.Translate (-this.transform.up / 10);
        }*/

        /*if (this.transform.position.y > +limit || this.transform.position.y < -limit) 
        {
            sense = !sense;
        }*/
    }
}