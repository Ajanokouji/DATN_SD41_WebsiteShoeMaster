import {
    Command,
    CommandEmpty,
    CommandGroup,
    CommandInput,
    CommandItem,
    CommandList,
  } from "@/components/ui/command";
  import { useAppDispatch } from "@/hooks/use-app-dispatch";
  import { useAppSelector } from "@/hooks/use-app-selector";
  import {
    selectCategories,
    // selectPagination,
  } from "@/redux/apps/category/categorySelector";
  import { fetchCategories } from "@/redux/apps/category/categorySlice";
  import { useEffect, useState } from "react";
  import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
  import { Button } from "@/components/ui/button";
  import { Check, ChevronsUpDown } from "lucide-react";
  
  interface ActionHeaderProps {
    onCategorySelect: (categoryId: string) => void;
  }
  
  const ActionHeader = ({ onCategorySelect }: ActionHeaderProps) => {
    const dispatch = useAppDispatch();
    const categories = useAppSelector(selectCategories);
    // const pagination = useAppSelector(selectPagination);
    const [searchValue, setSearchValue] = useState("");
    const [open, setOpen] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState<{
      id: string;
      name: string;
    } | null>(null);
  
    // Initial fetch
    useEffect(() => {
      dispatch(
        fetchCategories({
          CurrentPage: 1,
          PageSize: 15,
          Name: "",
        })
      );
    }, [dispatch]);
  
    // Fetch with search term when search value changes
    useEffect(() => {
      const debounceTimer = setTimeout(() => {
        dispatch(
          fetchCategories({
            CurrentPage: 1,
            PageSize: 15,
            Name: searchValue,
          })
        );
      }, 300); // Debounce search for 300ms
  
      return () => clearTimeout(debounceTimer);
    }, [searchValue, dispatch]);
  
    // Handle input change
    const handleSearchChange = (value: string) => {
        setSearchValue(value); // GUID mặc định
      
    };
  
    // Handle item selection
    const handleSelectCategory = (categoryId: string, categoryName: string) => {
      setSelectedCategory({ id: categoryId, name: categoryName });
      setOpen(false);
      
      // Send selected categoryId to parent component
      onCategorySelect(categoryId);
    };
  
    return (
      <section>
        <div className="grid grid-cols-2 justify-between">
          <div className="flex gap-2 items-center w-1/2">
            {/* Category Selector */}
            <Popover open={open} onOpenChange={setOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  aria-expanded={open}
                  className="w-full justify-between"
                >
                  {selectedCategory ? selectedCategory.name : "Chọn danh mục..."}
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-full p-0">
                <Command>
                  <CommandInput
                    placeholder="Search categories..."
                    value={""}
                    onValueChange={handleSearchChange}
                  />
                  <CommandList>
                    <CommandEmpty>No categories found.</CommandEmpty>
                    <CommandGroup heading="Categories">
                      {categories &&
                        categories.map((category) => (
                          <CommandItem
                            key={category.id}
                            onSelect={() => handleSelectCategory(category.id, category.name)}
                            className="flex items-center justify-between"
                          >
                            {category.name}
                            {selectedCategory?.id === category.id && (
                              <Check className="h-4 w-4" />
                            )}
                          </CommandItem>
                        ))}
                    </CommandGroup>
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          </div>
        </div>
      </section>
    );
  };
  
  export default ActionHeader;