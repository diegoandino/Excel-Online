#include "cell_value.h"

template<typename T>
inline CellValue<T>::CellValue() : content(NULL)
{
}

template<typename T>
CellValue<T>::CellValue(T& cont)  : content(cont)
{
}

template<typename T>
T CellValue<T>::get_content()
{
	return content;
}

template<typename T>
void CellValue<T>::set_content(T& cont)
{
	content = cont;
}

template<typename T>
bool CellValue<T>::is_empty()
{
	return content == NULL;
}


