﻿using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

/// <summary>
/// The model used for the BlazorBlocks editor.
/// It contains properties and methods for managing the editor rows and serializing/deserializing the model to JSON/HTML.
/// </summary>
public class BlazorBlocksEditorModel
{
    /// <summary>
    /// Gets or sets the list of editor row models.
    /// </summary>
    public List<EditorRowModel> Rows { get; set; }

    [JsonIgnore]
    public Action<string, string>? OnUpdated { get; set; }


    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorBlocksEditorModel"/> class.
    /// </summary>
    public BlazorBlocksEditorModel()
    {
        Rows = new List<EditorRowModel>();

        _jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = new JsonSerializerTypeResolver(BlockRegistrationService.RegisteredBlocks) };




        /*if(OnUpdated != null )
        {
            // when we're updated...
            OnUpdated(GetHTML(), GetJson());
        }*/
    }

    /// <summary>
    /// Gets the HTML representation of the editor model.
    /// </summary>
    /// <returns>The HTML string.</returns>
    public string GetHTML()
    {
        var sb = new StringBuilder();
        foreach (var row in Rows)
        {
            sb.Append(row.Render());
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets the JSON representation of the editor model.
    /// </summary>
    /// <returns>The JSON string.</returns>
    public string GetJson()
    {
        return JsonSerializer.Serialize(this, _jsonSerializerOptions);
    }

    /// <summary>
    /// Loads the editor model from the specified JSON string.
    /// </summary>
    /// <param name="data">The JSON string representing the editor model.</param>
    public void Load(string data)
    {
        var model = JsonSerializer.Deserialize<BlazorBlocksEditorModel>(data, _jsonSerializerOptions);
        Rows = model?.Rows ?? new List<EditorRowModel>();
    }

    public void MoveRow(EditorRowModel editorRowModel, int index)
    {
        Rows.Remove(editorRowModel);
        Rows.Insert(index, editorRowModel);
    }

    public void RemoveRow(EditorRowModel editorRowModel)
    {
        editorRowModel.OnUpdated = null;
        Rows.Remove(editorRowModel);
    }

    public void AddRow(EditorRowModel editorRowModel, int index)
    {
        // TODO: Add update tracking on row
        editorRowModel.OnUpdated = RowUpdated;
        Rows.Insert(index, editorRowModel);
    }

    private void RowUpdated()
    {
        OnUpdated?.Invoke(GetHTML(), GetJson());
    }
}
