# How to use?

## Li_Expandable component

### Example:

```html
<Li_Expandable
  Main="Data"
  Sublinks="Process settings, Production data, Tech data"
/>
```

`Li_Expandable` creates an expandable navigation link.

### Attributes:

- `Main`:

  - is the name of the the main link that upon click expands to sublinks...

  ![Main link](image.png)

- `Sublinks`

  ![Sublinks](image.png)

  - need to be separated by `", "` or `","`

### Important note:

After clicking on link `Process settings` the redirection uri will look like this: `/data/process_settings`. The main link is represented (in the uri) in lowercase and every sublink is in lowercase and `" "` in its name is replaced by `"_"`.
