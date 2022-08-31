# How to use?

## Li_Expandable component

### Example:

```xml
<Li_Expandable
  Main="Data"
  Sublinks="Process settings, Production data, Tech data"
/>
```

`Li_Expandable` creates an expandable navigation link.

### Attributes:

- `Main`:

  is the name of the the main link that upon click expands to sublinks...
  
  ![Main link](https://user-images.githubusercontent.com/87125815/187707814-e187ccde-0ddb-414b-9415-2e3c69c9d3ad.png)

- `Sublinks`

  need to be separated by `", "` or `","`
  
  ![Sublinks](https://user-images.githubusercontent.com/87125815/187707972-01ad8683-5aa1-44f4-8a0f-1175a20825ae.png)

### Important note:

After clicking on link `Process settings` the redirection uri will look like this: `/data/process_settings`. The main link is represented (in the uri) in lowercase and every sublink is in lowercase and `" "` in its name is replaced by `"_"`.
