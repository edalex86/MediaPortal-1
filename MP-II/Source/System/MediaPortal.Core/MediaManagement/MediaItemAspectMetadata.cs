#region Copyright (C) 2007-2009 Team MediaPortal

/*
    Copyright (C) 2007-2009 Team MediaPortal
    http://www.team-mediaportal.com
 
    This file is part of MediaPortal II

    MediaPortal II is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal II is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal II.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MediaPortal.Utilities.Xml;

namespace MediaPortal.Core.MediaManagement
{
  public enum Cardinality
  {
    /// <summary>
    /// The attribute is defined at the current media item aspect instance.
    /// </summary>
    /// <remarks>
    /// The attribute will be defined inline in the media item aspect's table.
    /// </remarks>
    Inline,

    /// <summary>
    /// There are multiple entries for the attribute, which are all dedicated to the
    /// current instance.
    /// </summary>
    /// <remarks>
    /// The attribute will be defined in its own table and the association attribute to the
    /// current media item aspect is located at the attribute's table.
    /// </remarks>
    OneToMany,

    /// <summary>
    /// There is exactly one associated entry, which is assigned to multiple
    /// media item's aspects.
    /// </summary>
    /// <remarks>
    /// The attribute will be defined in its own table and the association attribute to the
    /// attribute's value is defined in the media item aspect's table.
    /// </remarks>
    ManyToOne,

    /// <summary>
    /// There are multiple entries for the attribute, which are not dedicated to the
    /// current instance.
    /// </summary>
    /// <remarks>
    /// The attribute will be defined in its own table and the association between the
    /// media item aspect's table and the attribute's table will be defined in its own N:M table.
    /// </remarks>
    ManyToMany
  }

  /// <summary>
  /// Metadata descriptor for a <see cref="MediaItemAspect"/>.
  /// Once a <see cref="MediaItemAspectMetadata"/> is released with a given <see cref="AspectId"/>, it must not
  /// be changed any more. Updating a <see cref="MediaItemAspectMetadata"/> must lead to a change of the
  /// <see cref="AspectId"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note: This class is serialized/deserialized by the <see cref="XmlSerializer"/>.
  /// If changed, this has to be taken into consideration.
  /// </para>
  /// </remarks>
  public class MediaItemAspectMetadata
  {
    #region Constants

    /// <summary>
    /// Contains a collection of supported basic types. Together with those basic types, the <see cref="string"/> type
    /// is also supported.
    /// </summary>
    /// <remarks>
    /// The following types are supported:
    /// <list>
    /// <item><see cref="DateTime"/></item>
    /// <item><see cref="Char"/></item>
    /// <item><see cref="Boolean"/></item>
    /// <item><see cref="Single"/></item>
    /// <item><see cref="Double"/></item>
    /// <item><see cref="Int32"/></item>
    /// <item><see cref="Int64"/></item>
    /// </list>
    /// </remarks>
    public static readonly ICollection<Type> SUPPORTED_BASIC_TYPES = new List<Type>
        {
          typeof(DateTime),
          typeof(Char),
          typeof(Boolean),
          typeof(Single),
          typeof(Double),
          typeof(Int32),
          typeof(Int64)
        };

    #endregion

    /// <summary>
    /// Stores the metadata for one attribute in a media item aspect.
    /// </summary>
    public class AttributeSpecification
    {
      #region Protected fields

      protected MediaItemAspectMetadata _parentMIAM = null;
      protected string _attributeName;
      protected Type _attributeType;
      protected Cardinality _cardinality;
      protected uint _maxNumChars;

      #endregion

      internal AttributeSpecification(string name, Type type, Cardinality cardinality)
      {
        _attributeName = name;
        _attributeType = type;
        _cardinality = cardinality;
      }

      /// <summary>
      /// Gets or sets the media item aspect metadata instance, this attribute type belongs to.
      /// </summary>
      [XmlIgnore]
      public MediaItemAspectMetadata ParentMIAM
      {
        get { return _parentMIAM; }
        internal set { _parentMIAM = value; }
      }

      /// <summary>
      /// Name of the defined attribute. The name is unique among all attributes of the same
      /// media item aspect.
      /// </summary>
      [XmlIgnore]
      public string AttributeName
      {
        get { return _attributeName; }
      }

      /// <summary>
      /// Runtime value type for the attribute instances.
      /// </summary>
      [XmlIgnore]
      public Type AttributeType
      {
        get { return _attributeType; }
      }

      /// <summary>
      /// Gets the cardinality of this attribute. See the docs for the
      /// <see cref="Cardinality"/> class and its members.
      /// </summary>
      [XmlIgnore]
      public Cardinality Cardinality
      {
        get { return _cardinality; }
      }

      /// <summary>
      /// Returns the information if this attribute is a collection attribute, i.e. supports multi-value
      /// entries.
      /// </summary>
      [XmlIgnore]
      public bool IsCollectionAttribute
      {
        get { return _cardinality == Cardinality.ManyToMany || _cardinality == Cardinality.OneToMany; }
      }

      /// <summary>
      /// If this is a string attribute type, this property gets or sets the maximum number of characters which can be
      /// stored in an attribute instance of this type.
      /// </summary>
      [XmlIgnore]
      public uint MaxNumChars
      {
        get { return _maxNumChars; }
        set { _maxNumChars = value; }
      }

      #region Additional members for the XML serialization

      internal AttributeSpecification() { }

      /// <summary>
      /// For internal use of the XML serialization system only.
      /// </summary>
      [XmlElement("AttributeName")]
      public string XML_AttributeName
      {
        get { return _attributeName; }
        set { _attributeName = value; }
      }

      /// <summary>
      /// For internal use of the XML serialization system only.
      /// </summary>
      [XmlElement("AttributeType")]
      public string XML_AttributeType
      {
        get { return _attributeType.FullName; }
        set { _attributeType = Type.GetType(value); }
      }

      /// <summary>
      /// For internal use of the XML serialization system only.
      /// </summary>
      [XmlElement("Cardinality")]
      public Cardinality XML_Cardinality
      {
        get { return _cardinality; }
        set { _cardinality = value; }
      }

      /// <summary>
      /// For internal use of the XML serialization system only.
      /// </summary>
      [XmlElement("MaxNumChars")]
      public uint XML_MaxNumChars
      {
        get { return _maxNumChars; }
        set { _maxNumChars = value; }
      }

      #endregion
    }

    #region Protected fields

    protected string _aspectName;
    protected Guid _aspectId;
    protected bool _isSystemAspect;
    protected IDictionary<string, AttributeSpecification> _attributeSpecifications =
        new Dictionary<string, AttributeSpecification>();

    // We could use some cache for this instance, if we would have one...
    [ThreadStatic]
    protected static XmlSerializer _xmlSerializer = null; // Lazy initialized

    #endregion

    /// <summary>
    /// Creates a new aspect metadata instance. An aspect with a given <paramref name="aspectId"/>
    /// SHOULD NOT be initialized from multiple code parts by other modules than the MediaPortal Core,
    /// i.e. plugins creating a specified media item aspect should create and exposed it to the entire system
    /// from a dedicated constant class.
    /// </summary>
    /// <param name="aspectId">Unique id of the new aspect type to create.</param>
    /// <param name="aspectName">Name of the new aspect type. The name should be unique and may be
    /// a localized string.</param>
    /// <param name="attributeSpecifications">Enumeration of specifications for the attribute of the new
    /// aspect type</param>
    public MediaItemAspectMetadata(Guid aspectId, string aspectName,
        IEnumerable<AttributeSpecification> attributeSpecifications)
    {
      _aspectId = aspectId;
      _aspectName = aspectName;
      foreach (AttributeSpecification spec in attributeSpecifications)
        _attributeSpecifications[spec.AttributeName] = spec;
      CorrectParentsInAttributeTypes();
    }

    protected void CorrectParentsInAttributeTypes()
    {
      foreach (AttributeSpecification attributeType in _attributeSpecifications.Values)
        attributeType.ParentMIAM = this;
    }

    /// <summary>
    /// Returns the globally unique ID of this aspect.
    /// </summary>
    [XmlIgnore]
    public Guid AspectId
    {
      get { return _aspectId; }
    }

    /// <summary>
    /// Name of this aspect. Can be shown in the gui, for example. The returned string may be
    /// a localized string label (i.e. "[section.name]").
    /// </summary>
    [XmlIgnore]
    public string Name
    {
      get { return _aspectName; }
    }

    /// <summary>
    /// Gets or sets the information if this aspect is a system aspect. System aspects must not be deleted.
    /// This property can only be set internal.
    /// </summary>
    [XmlIgnore]
    public bool IsSystemAspect
    {
      get { return _isSystemAspect; }
      internal set { _isSystemAspect = value; }
    }

    /// <summary>
    /// Returns a read-only mapping of names to attribute specifications for all
    /// available attributes of this media item aspect.
    /// </summary>
    [XmlIgnore]
    public IDictionary<string, AttributeSpecification> AttributeSpecifications
    {
      get { return _attributeSpecifications; }
    }

    /// <summary>
    /// Creates a specification for a new string attribute which can be used in a new
    /// <see cref="MediaItemAspectMetadata"/> instance.
    /// </summary>
    /// <param name="attributeName">Name of the string attribute to be created.
    /// TODO: Describe constraints on the name value. See also other CreateXXXAttributeSpecification methods.</param>
    /// <param name="maxNumChars">Maximum number of characters to be stored in the attribute to
    /// be created.</param>
    /// <param name="cardinality">Cardinality of the new attribute.</param>
    public static AttributeSpecification CreateStringAttributeSpecification(string attributeName,
        uint maxNumChars, Cardinality cardinality)
    {
      AttributeSpecification result = new AttributeSpecification(attributeName, typeof(string), cardinality)
        {MaxNumChars = maxNumChars};
      return result;
    }

    /// <summary>
    /// Creates a specification for a new attribute which can be used in a new
    /// <see cref="MediaItemAspectMetadata"/> instance.
    /// </summary>
    /// <param name="attributeName">Name of the string attribute to be created.
    /// TODO: Describe constraints on the name value. See also other CreateXXXAttributeSpecification methods.</param>
    /// <param name="attributeType">Type of the attribute to be stored in the attribute to be created.
    /// For a list of supported attribute types, see <see cref="SUPPORTED_BASIC_TYPES"/>.</param>
    /// <param name="cardinality">Cardinality of the new attribute.</param>
    public static AttributeSpecification CreateAttributeSpecification(string attributeName,
        Type attributeType, Cardinality cardinality)
    {
      if (!SUPPORTED_BASIC_TYPES.Contains(attributeType))
        throw new ArgumentException(string.Format("Attribute type {0} is not supported for media item aspect attributes",
            attributeType.Name));
      return new AttributeSpecification(attributeName, attributeType, cardinality);
    }

    /// <summary>
    /// Serializes this MediaItem aspect metadata instance to XML.
    /// </summary>
    /// <returns>String containing an XML fragment with this instance's data.</returns>
    public string Serialize()
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      lock (xs)
      {
        StringBuilder sb = new StringBuilder(); // Will contain the data, formatted as XML
        using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings {OmitXmlDeclaration = true}))
          xs.Serialize(writer, this);
        return sb.ToString();
      }
    }

    /// <summary>
    /// Serializes this MediaItem aspect metadata instance to the given <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">Writer to write the XML serialization to.</param>
    public void Serialize(XmlWriter writer)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      lock (xs)
        xs.Serialize(writer, this);
    }

    /// <summary>
    /// Deserializes a MediaItem aspect metadata instance from a given XML fragment.
    /// </summary>
    /// <param name="str">XML fragment containing a serialized MediaItem aspect metadata instance.</param>
    /// <returns>Deserialized instance.</returns>
    public static MediaItemAspectMetadata Deserialize(string str)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      lock (xs)
        using (StringReader reader = new StringReader(str))
          return xs.Deserialize(reader) as MediaItemAspectMetadata;
    }

    /// <summary>
    /// Deserializes a MediaItem aspect metadata instance from a given <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">XML reader containing a MediaItem aspect metadata XML serialization.</param>
    /// <returns>Deserialized instance.</returns>
    public static MediaItemAspectMetadata Deserialize(XmlReader reader)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      lock (xs)
        return xs.Deserialize(reader) as MediaItemAspectMetadata;
    }

    public override bool Equals(object obj)
    {
      MediaItemAspectMetadata other = obj as MediaItemAspectMetadata;
      if (other == null)
        return false;
      return other.AspectId == _aspectId;
    }

    public override int GetHashCode()
    {
      return _aspectId.GetHashCode();
    }

    public override string ToString()
    {
      return "MIA Type '" + _aspectName + "' (Id='" + _aspectId + "')";
    }

    #region Additional members for the XML serialization

    internal MediaItemAspectMetadata() { }

    protected static XmlSerializer GetOrCreateXMLSerializer()
    {
      if (_xmlSerializer == null)
        _xmlSerializer = new XmlSerializer(typeof(MediaItemAspectMetadata));
      return _xmlSerializer;
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("Id")]
    public string XML_AspectId
    {
      get { return _aspectId.ToString(); }
      set { _aspectId = new Guid(value); }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("IsSystemAspect")]
    public bool XML_IsSystemAspect
    {
      get { return _isSystemAspect; }
      set { _isSystemAspect = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlElement("Name")]
    public string XML_Name
    {
      get { return _aspectName; }
      set { _aspectName = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlArray("Attributes")]
    [XmlArrayItem("AttributeType")]
    public List<AttributeSpecification> XML_AttributeSpecifications
    {
      get { return new List<AttributeSpecification>(_attributeSpecifications.Values); }
      set
      {
        _attributeSpecifications = new Dictionary<string, AttributeSpecification>(value.Count);
        foreach (AttributeSpecification spec in value)
          _attributeSpecifications.Add(spec.AttributeName, spec);
        CorrectParentsInAttributeTypes();
      }
    }

    #endregion
  }
}
