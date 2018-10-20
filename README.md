# booksearch

This repository contains a web application "WebApplication2", which contains both web api and mvc web application.

Instruction for running the application:
1. When you start the application, you will come to the booksearch site where you can see all the books.
2. There are four different filters you can use to search for books: Title, Author, Genre and Price. Title and Author filters can be used to serach books with free keywords. Genre is a dropdown list that shows all the available genres. Price is a dropdown list tha tshows different price ranges. 
3. For title and author fitler, there are some rules implemented:
	a. Cretain symbols in serach strings will be ignored. (",", ".", "/", "\\", "-", "=", "@", "&", "%")  e.g. If you search for author name "joe , kutner", you will get match for author name that contains "joe" and "kutner", and "," will not be considered necessary to match the serach string. 
	b. If you repeat the same word multiple times in your serach string, it is conted once. E.g. if you serach for title with "the the", the book(s) returned should not have contain two "the" in the tile, it is enough that title contains one "the". 
	c. Serach result should return items that contain the search string. E.g. if you serach for title word "Ruby", even a book that contains the word "JRuby" should be returned.
	d. If you write multiple search words separated by white space, only the items that contain all those serach words should be returned. 
4. There is no button to remove all filters. if you want to show all the books again after your serach, you need to remove all the filters yourself and klick "Filter" button.