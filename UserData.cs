/**********************************************************************************************
* File:         UserData.cs                                                                   *
* Contents:     Class UserData                                                                *
* Author:       Alex Konnen (alex@esquisse-studio.eu)                                         *
* Date:         2013-04-06 12:40                                                              *
* Version:      1.0                                                                           *
* Copyright:    Esquisse Laboratories (http://www.esquisse-studio.eu/)                        *
**********************************************************************************************/
using Microsoft.Win32;
using System;
using System.Xml.Linq;

namespace FlowerPot
{
	public class UserData
	{
		/// <summary>
		///		Name of the company whose registry should be used.
		/// </summary>
		/// <value>
		///		The name of the global company.
		/// </value>
		public static string GlobalCompanyName	{get;set;}	= "Pikkatech";

		/// <summary>
		/// Gets or sets the name of the product.
		/// </summary>
		/// <value>
		/// The name of the product.
		/// </value>
		public static string GlobalProductName	{get;set;}	= "FlowerPotConfigurator";

		#region Data
		public string	AuthorName		{get;set;}
		public string	AuthorEmail		{get;set;}

		/// <summary>
		/// The company to which the author belongs.
		/// </summary>
		/// <value>
		/// The name of the company.
		/// </value>
		public string	CompanyName		{get;set;}
		public string	CompanyWebsite	{get;set;}
		public int		HeaderWidth		{get;set;}	= 84;
		#endregion

		#region Constructor
		public UserData(string authorName, string authorEmail, string companyName, string companyWebsite)
		{
			this.AuthorEmail	= authorEmail;
			this.AuthorEmail	= authorName;
			this.CompanyName	= companyName;
			this.CompanyWebsite	= companyWebsite;
		}

		public UserData()	: this("Unknown Author", "unknown@unknown.un", "Unknown Company", "www.unknown.un")	{}
		#endregion

		#region XML
		public XElement ToXElement()
		{
			XElement x	= new XElement("User", new XAttribute("Source", this.CompanyName));

			x.Add(new XElement("AuthorName",		this.AuthorName));
			x.Add(new XElement("AuthorEmail",		this.AuthorEmail));
			x.Add(new XElement("CompanyName",		this.CompanyName));
			x.Add(new XElement("CompanyWebsite",	this.CompanyWebsite));
			x.Add(new XElement("HeaderWidth",		this.HeaderWidth));

			return x;
		}

		public static UserData FromXElement(XElement x)
		{
			UserData ud			= new UserData();

			ud.AuthorName		= x.Element("AuthorName").Value;
			ud.AuthorEmail		= x.Element("AuthorEmail").Value;
			ud.CompanyName		= x.Element("CompanyName").Value;
			ud.CompanyWebsite	= x.Element("CompanyWebsite").Value;

			try
			{
				ud.HeaderWidth		= Int32.Parse(x.Element("HeaderWidth").Value);
			}
			catch {}

			return ud;
		}
		#endregion

		#region Registry
		public static UserData FromRegistry(string registryKey)
		{
			RegistryKey regKey		= Registry.CurrentUser;
			regKey					= regKey.CreateSubKey(registryKey);

			try
			{
				UserData userData		= new UserData();

				userData.AuthorName		= regKey.GetValue("AuthorName",		userData.AuthorName).ToString();
				userData.AuthorEmail	= regKey.GetValue("AuthorEmail",	userData.AuthorEmail).ToString();
				userData.CompanyName	= regKey.GetValue("CompanyName",	userData.CompanyName).ToString();
				userData.CompanyWebsite	= regKey.GetValue("CompanyWebsite",	userData.CompanyWebsite).ToString();
				userData.HeaderWidth	= Int32.Parse(regKey.GetValue("HeaderWidth",	userData.HeaderWidth).ToString());

				return userData;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void ToRegistry(string registryKey)
		{
			RegistryKey regKey					= Registry.CurrentUser;
			regKey								= regKey.CreateSubKey(registryKey);

			regKey.SetValue("AuthorName",		this.AuthorName);
			regKey.SetValue("AuthorEmail",		this.AuthorEmail);
			regKey.SetValue("CompanyName",		this.CompanyName);
			regKey.SetValue("CompanyWebsite",	this.CompanyWebsite);
			regKey.SetValue("HeaderWidth",		this.HeaderWidth);
		}
		#endregion

		public override string ToString()
		{
			return $"{this.AuthorName} ({this.AuthorEmail})";
		}
	}
}
