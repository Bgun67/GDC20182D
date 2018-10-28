using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class WeaponLoader{
	public static string[] loadoutData;
	// Use this for initialization
	public static void LoadWeaponData()
	{
		try
		{
			loadoutData = File.ReadAllLines(@"Loadout Data.txt");
		}
		catch
		{
			loadoutData = new string[] { "Sword","Sword" };
			File.WriteAllLines(@"Loadout Data.txt", loadoutData);

		}

	}
	public static Weapon LoadWeapon(Transform _finger, int weaponNumber)
	{
		
		LoadWeaponData();
		try
		{
			string data = loadoutData[weaponNumber - 1];
		}
		catch
		{
			loadoutData = new string[] { "Sword","Sword" };
			File.WriteAllLines(@"Loadout Data.txt", loadoutData);
		}


		GameObject _primaryWeaponPrefab = (GameObject)(Resources.Load(loadoutData[weaponNumber-1]));
		_primaryWeaponPrefab.SetActive(false);

		
		Weapon _weaponScript = _primaryWeaponPrefab.GetComponent<Weapon>();

		//Quaternion _rotation = _finger.rotation + 
		GameObject _primaryWeapon = GameObject.Instantiate(_primaryWeaponPrefab);
		_primaryWeapon.transform.position += _finger.position -_weaponScript.grip.position;
		_primaryWeapon.transform.rotation = _finger.transform.rotation;
		_primaryWeapon.transform.parent = _finger;
		_primaryWeapon.SetActive(true);
		return _primaryWeapon.GetComponent<Weapon>();

	}
}
